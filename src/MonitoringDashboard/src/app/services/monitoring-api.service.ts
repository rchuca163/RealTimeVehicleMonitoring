import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { VehicleSnapshot } from '../models/vehicle-snapshot';

@Injectable({
  providedIn: 'root',
})
export class MonitoringApiService {
  private readonly http = inject(HttpClient);

  private readonly baseUrl = 'https://localhost:7135/api/Monitoring';

  getVehicleSnapshot(companyName?: string, plate?: string): Observable<VehicleSnapshot[]> {
    let params = new HttpParams();

    if (companyName?.trim()) {
      params = params.set('companyName', companyName.trim());
    }

    if (plate?.trim()) {
      params = params.set('plate', plate.trim());
    }

    return this.http.get<VehicleSnapshot[]>(
      `${this.baseUrl}/vehicles`,
      { params },
    );
  }
}