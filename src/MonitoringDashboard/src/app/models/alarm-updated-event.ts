import { VehicleAlarms } from "./vehicle-snapshot";

export interface AlarmUpdatedEvent {
  vehicleId: number;
  alarms: VehicleAlarms;
}