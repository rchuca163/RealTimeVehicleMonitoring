export interface VehicleSnapshot {
  vehicleId: number;
  companyId: number;
  companyName: string;
  plate: string;

  positionId: number | null;
  latitude: number | null;
  longitude: number | null;
  speed: number | null;

  alarms: VehicleAlarms | null;
}

export interface VehicleAlarms {
  panic: boolean;
  speeding: boolean;
  stopped: boolean;
}