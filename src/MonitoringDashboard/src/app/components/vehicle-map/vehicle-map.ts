import {
  AfterViewInit,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnDestroy,
  Output,
  ViewChild,
} from '@angular/core';

import maplibregl from 'maplibre-gl';
import { VehicleSnapshot } from '../../models/vehicle-snapshot';
import { VEHICLE_STATUS } from '../../shared/vehicle-status';

@Component({
  imports: [],
  selector: 'app-vehicle-map',
  styleUrl: './vehicle-map.css',
  templateUrl: './vehicle-map.html',
})
export class VehicleMap implements AfterViewInit, OnDestroy {
  @ViewChild('mapContainer', { static: true })
  private readonly mapContainer!: ElementRef<HTMLDivElement>;

  @Output()
  vehicleSelected = new EventEmitter<VehicleSnapshot>();

  private _vehicles: VehicleSnapshot[] = [];

  private _darkTheme = true;
  
  @Input()
  set darkTheme(value: boolean) {
    this._darkTheme = value;
  
    if (!this.map) {
      return;
    }
  
    this.changeMapStyle();
  }

  /**
   * Reloads the map style when the application theme changes.
   *
   * The style reload removes custom layers, so the 3D buildings
   * need to be added again after the new style finishes loading.
   */
  private changeMapStyle(): void {
    if (!this.map) {
      return;
    }
  
    const style = this._darkTheme
      ? 'https://tiles.openfreemap.org/styles/dark'
      : 'https://tiles.openfreemap.org/styles/bright';
  
    this.map.setStyle(style);
    
    this.map.once('styledata', () => {
      this.add3DBuildings();
      this.updateMarkers(false);
    });
  }

  @Input()
  set vehicles(value: VehicleSnapshot[]) {
    this._vehicles = value;
    this.updateMarkers();
  }
  
  get vehicles(): VehicleSnapshot[] {
    return this._vehicles;
  }

  private readonly animationDuration = 800;
  private map?: maplibregl.Map;
  private readonly markers = new Map<number, maplibregl.Marker>();

  private readonly singleVehicleMaxZoom = 16;

  ngOnDestroy(): void {
    this.map?.remove();
  }

  ngAfterViewInit(): void {
    this.map = new maplibregl.Map({
      container: this.mapContainer.nativeElement,
      style: this._darkTheme      
        ? 'https://tiles.openfreemap.org/styles/dark'
        : 'https://tiles.openfreemap.org/styles/bright',
      center: [-3.7038, 40.4200],
      zoom: 14.5,
      pitch: 52,
      bearing: -10,
      antialias: true,
    });

    this.map.addControl(
      new maplibregl.NavigationControl({
        visualizePitch: true,
      }),
      'bottom-left',
    );

    this.map.on('load', () => {
      this.add3DBuildings();
      this.updateMarkers();
      this.fitMapToVehicles();
    });
  }

  /**
   * Synchronizes map markers with the current vehicle collection.
   *
   * When shouldFitMap is false, markers are updated without changing
   * the current camera position. This is used when switching map themes.
   */
  private updateMarkers(shouldFitMap = true): void {  
    if (!this.map) {
      return;
    }
  
    const currentVehicleIds = new Set(
      this.vehicles.map(vehicle => vehicle.vehicleId),
    );
  
    // Eliminar marcadores de vehículos que ya no forman parte del filtro.
    for (const [vehicleId, marker] of this.markers) {
      if (!currentVehicleIds.has(vehicleId)) {
        marker.remove();
        this.markers.delete(vehicleId);
      }
    }
  
    // Crear o actualizar marcadores.
    for (const vehicle of this.vehicles) {
      if (vehicle.latitude === null || vehicle.longitude === null) {
        continue;
      }
  
      const existingMarker = this.markers.get(vehicle.vehicleId);
  
      if (existingMarker) {
        this.animateMarkerTo(existingMarker, vehicle.longitude, vehicle.latitude);
        
        this.updateVehicleMarkerColor(existingMarker, vehicle);
        continue;
      }
  
      const marker = new maplibregl.Marker()
        .setLngLat([vehicle.longitude, vehicle.latitude])
        .addTo(this.map);
      
      this.updateVehicleMarkerColor(marker, vehicle);

      marker.getElement().addEventListener('click', () => {
        this.vehicleSelected.emit(vehicle);
      });

      this.markers.set(vehicle.vehicleId, marker);
    }
  
    if (shouldFitMap) {
      this.fitMapToVehicles();
    }
  }

  private updateVehicleMarkerColor(
    marker: maplibregl.Marker,
    vehicle: VehicleSnapshot
  ): void {
    const color = this.getVehicleMarkerColor(vehicle);
  
    const path = marker.getElement().querySelector('svg path');
  
    path?.setAttribute('fill', color);
  }

  private getVehicleMarkerColor(vehicle: VehicleSnapshot): string {
    if (vehicle.alarms?.panic) {
      return VEHICLE_STATUS.panic.color;
    }
  
    if (vehicle.alarms?.speeding) {
      return VEHICLE_STATUS.speeding.color;
    }
  
    if (vehicle.alarms?.stopped) {
      return VEHICLE_STATUS.stopped.color;
    }
  
    return VEHICLE_STATUS.normal.color;
  }

  /**
   * Adjusts the map camera so all vehicles with a valid position
   * are visible within the map viewport.
   */
  private fitMapToVehicles(): void {
    if (!this.map) {
      return;
    }
  
    const vehiclesWithPosition = this.vehicles.filter(
      (
        vehicle,
      ): vehicle is VehicleSnapshot & {
        latitude: number;
        longitude: number;
      } =>
        vehicle.latitude !== null &&
        vehicle.longitude !== null,
    );
  
    if (vehiclesWithPosition.length === 0) {
      return;
    }

    if (vehiclesWithPosition.length === 1) {
      const vehicle = vehiclesWithPosition[0];
    
      console.log("zoom" + Math.max(this.map.getZoom(), this.singleVehicleMaxZoom));     
    
      this.map.easeTo({
        center: [vehicle.longitude, vehicle.latitude],
        zoom: Math.max(this.map.getZoom(), this.singleVehicleMaxZoom),
        duration: this.animationDuration,
      });
    
      return;
    }
  
    const bounds = new maplibregl.LngLatBounds();
  
    for (const vehicle of vehiclesWithPosition) {
      bounds.extend([
        vehicle.longitude,
        vehicle.latitude,
      ]);
    }
  
    this.map.fitBounds(bounds, {
        padding: this.getMapFitPadding(),
        maxZoom: 14,
        duration: this.animationDuration
      });
  }

  /**
   * Calculates responsive padding for fitBounds so that vehicles
   * remain visible without being hidden behind the dashboard UI.
   */
  private getMapFitPadding(): maplibregl.PaddingOptions {
    const mapWidth = this.map!.getContainer().clientWidth;
    const mapHeight = this.map!.getContainer().clientHeight;
  
    const horizontalPadding = Math.min(
      300,
      Math.max(200, mapWidth * 0.16),
    );
  
    const verticalPadding = Math.min(
      140,
      Math.max(100, mapHeight * 0.14),
    );
  
    return {
      top: verticalPadding,
      bottom: verticalPadding,
      left: horizontalPadding,
      right: horizontalPadding,
    };
  }

  /**
   * Smoothly moves an existing marker from its current position
   * to the vehicle's latest GPS coordinates.
   */
  private animateMarkerTo(
    marker: maplibregl.Marker,
    longitude: number,
    latitude: number
  ): void {
    const start = marker.getLngLat();
    const startTime = performance.now();
  
    const animate = (currentTime: number) => {
      const elapsed = currentTime - startTime;
      const progress = Math.min(elapsed / this.animationDuration, 1);
  
      const currentLongitude =
        start.lng + (longitude - start.lng) * progress;
  
      const currentLatitude =
        start.lat + (latitude - start.lat) * progress;
  
      marker.setLngLat([currentLongitude, currentLatitude]);
  
      if (progress < 1) {
        requestAnimationFrame(animate);
      }
    };
  
    requestAnimationFrame(animate);
  }

  /**
   * Adds the custom 3D building layer to the map.
   *
   * The layer is recreated after every style change because
   * MapLibre removes custom layers when a new style is loaded.
   */
  private add3DBuildings(): void {
    if (!this.map) {
      return;
    }

    const map = this.map;

    if (!map.getSource('openmaptiles')) {
      return;
    }

    if (map.getLayer('3d-buildings')) {
      return;
    }

    for (const id of [
      'building',
      'building-outline',
      'building-top',
    ]) {
      if (map.getLayer(id)) {
        map.setLayoutProperty(id, 'visibility', 'none');
      }
    }

    const buildingColor: maplibregl.ExpressionSpecification = this._darkTheme
      ? [
          'interpolate',
          ['linear'],
          ['get', 'render_height'],
          0, '#141e30',
          20, '#253d5a',
          50, '#3a5f82',
        ]
        // [
        //   'interpolate',
        //   ['linear'],
        //   ['get', 'render_height'],
        //   0, '#141e30',
        //   15, '#1c2d45',
        //   40, '#253d5a',
        //   80, '#2e4d6e',
        //   150, '#3a5f82',
        // ]
      : [
          'case',
          ['has', 'colour'],
          ['get', 'colour'],
          'hsl(39, 41%, 86%)',
        ];

    map.addLayer({
      id: '3d-buildings',
      source: 'openmaptiles',
      'source-layer': 'building',
      type: 'fill-extrusion',
      minzoom: 13,
      paint: {
        'fill-extrusion-color': buildingColor,
        'fill-extrusion-height': [
          'coalesce',
          ['get', 'render_height'],
          8,
        ],
        'fill-extrusion-base': [
          'coalesce',
          ['get', 'render_min_height'],
          0,
        ],
        'fill-extrusion-opacity': 0.9,
      },
    });

    // Keep map labels above the 3D buildings so cities, streets,
    // and other labels remain visible.
    const firstSymbolLayer = map
      .getStyle()
      .layers
      .find(layer => layer.type === 'symbol');
    
    if (firstSymbolLayer) {
      map.moveLayer('3d-buildings', firstSymbolLayer.id);
    }

  }
}