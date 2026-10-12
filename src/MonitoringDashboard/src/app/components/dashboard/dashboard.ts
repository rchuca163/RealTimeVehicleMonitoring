import { Component, OnDestroy, OnInit, computed, inject, signal } from '@angular/core';
import { VehicleSnapshot } from '../../models/vehicle-snapshot';
import { MonitoringApiService } from '../../services/monitoring-api.service';
import { MonitoringSignalRService } from '../../services/monitoring-signalr.service';
import { VehicleMap } from '../vehicle-map/vehicle-map';
import { VEHICLE_STATUS } from '../../shared/vehicle-status';
import { finalize } from 'rxjs';

@Component({
  imports: [VehicleMap],
  selector: 'app-dashboard',
  styleUrl: './dashboard.css',
  templateUrl: './dashboard.html',
})
export class Dashboard implements OnInit, OnDestroy {
  private readonly monitoringApi = inject(MonitoringApiService);
  private readonly monitoringSignalR = inject(MonitoringSignalRService);

  protected readonly vehicles = signal<VehicleSnapshot[]>([]);
  protected readonly companyFilter = signal('');
  protected readonly plateFilter = signal('');
  protected readonly selectedVehicleId = signal<number | null>(null);
  protected readonly isLoading = signal(true);
  protected readonly isDarkTheme = signal(false);

  protected readonly vehicleStatus = VEHICLE_STATUS;
  
  private readonly subscribedVehicleIds = new Set<number>();

  private filterTimer?: ReturnType<typeof setTimeout>;

  protected readonly selectedVehicle = computed(() => {
    const vehicleId = this.selectedVehicleId();
  
    if (vehicleId === null) {
      return null;
    }
  
    return this.vehicles().find(
      vehicle => vehicle.vehicleId === vehicleId,
    ) ?? null;
  });

  protected readonly hasVehicles = computed(() =>
    this.vehicles().length > 0
  );
  
  protected readonly hasVehiclesWithPosition = computed(() =>
    this.vehicles().some(
      vehicle =>
        vehicle.latitude !== null &&
        vehicle.longitude !== null
    )
  );

  protected readonly vehiclesWithPositionCount = computed(() =>
    this.vehicles().filter(
      vehicle => vehicle.latitude !== null && vehicle.longitude !== null
    ).length
  );

  protected toggleTheme(): void {
    this.isDarkTheme.update(isDark => {
      const newIsDarkTheme = !isDark;
  
      document.documentElement.dataset['theme'] =
        newIsDarkTheme ? 'dark' : 'light';
  
      return newIsDarkTheme;
    });
  }

  ngOnInit(): void {
    document.documentElement.dataset['theme'] = 'light';

    this.initialize();
  }

  ngOnDestroy(): void {
    if (this.filterTimer) {
      clearTimeout(this.filterTimer);
    }
  }

  formatCoordinate(value: number | null | undefined): string {
    return value != null ? value.toFixed(4) : '-';
  }

  private async initialize(): Promise<void> {
    await this.connectToSignalR();
    this.loadVehicleSnapshot();
  }

  private loadVehicleSnapshot(): void {
    this.isLoading.set(true);
    
    this.monitoringApi.getVehicleSnapshot(
      this.companyFilter(),
      this.plateFilter()
      )
      .pipe(
        finalize(() => this.isLoading.set(false))
      )
      .subscribe({
        next: vehicles => {
          this.updateVehicleGroups(vehicles);  
          this.vehicles.set(vehicles);
        },
        error: error => {
          console.error('Error loading vehicle snapshot:', error);
        },
      });
  }

  private updateVehicleGroups(vehicles: VehicleSnapshot[]): void {
    const newVehicleIds = new Set(
      vehicles.map(vehicle => vehicle.vehicleId),
    );
  
    for (const vehicleId of this.subscribedVehicleIds) {
      if (!newVehicleIds.has(vehicleId)) {
        this.monitoringSignalR.leaveVehicleGroup(vehicleId);
        this.subscribedVehicleIds.delete(vehicleId);
      }
    }
  
    for (const vehicle of vehicles) {
      if (this.subscribedVehicleIds.has(vehicle.vehicleId)) {
        continue;
      }
  
      this.monitoringSignalR.joinVehicleGroup(vehicle.vehicleId);
      this.subscribedVehicleIds.add(vehicle.vehicleId);
    }
  }

  private async connectToSignalR(): Promise<void> {
    this.monitoringSignalR.onTestMessage(message => {
      console.log('SignalR TestMessage:', message);
    });
  
    this.monitoringSignalR.onPositionUpdated(event => {
      console.log('SignalR PositionUpdated:', event);
    
      this.vehicles.update(vehicles =>
        vehicles.map(vehicle =>
          vehicle.vehicleId === event.vehicleId
            ? {
                ...vehicle,
                positionId: event.positionId,
                latitude: event.latitude,
                longitude: event.longitude,
                speed: event.speed,
              }
            : vehicle
        )
      );
    });

    this.monitoringSignalR.onAlarmUpdated(event => {
      console.log('SignalR AlarmUpdated:', event);
    
      this.vehicles.update(vehicles =>
        vehicles.map(vehicle =>
          vehicle.vehicleId === event.vehicleId
            ? {
                ...vehicle,
                alarms: event.alarms,
              }
            : vehicle
        )
      );
    });
    
    this.monitoringSignalR.onReconnected(() => {
      this.resubscribeToVehicles();
    });

    await this.monitoringSignalR.start();
  }

  private resubscribeToVehicles(): void {
    for (const vehicleId of this.subscribedVehicleIds) {
      this.monitoringSignalR.joinVehicleGroup(vehicleId);
    }
  }

  protected onPlateFilterChange(event: Event): void {
    const input = event.target as HTMLInputElement;

    this.plateFilter.set(input.value);
    this.scheduleVehicleSnapshot();
  }
  
  protected onCompanyFilterChange(event: Event): void {
    const input = event.target as HTMLInputElement;
  
    this.companyFilter.set(input.value);
    this.scheduleVehicleSnapshot();
  }

  private scheduleVehicleSnapshot(): void {
    if (this.filterTimer) {
      clearTimeout(this.filterTimer);
    }
  
    this.filterTimer = setTimeout(() => {
      this.loadVehicleSnapshot();
    }, 600);
  }

  protected onVehicleSelected(vehicle: VehicleSnapshot): void {
    this.selectedVehicleId.set(vehicle.vehicleId);
  }

  protected closeVehicleDetail(): void {
    this.selectedVehicleId.set(null);
  }
}
