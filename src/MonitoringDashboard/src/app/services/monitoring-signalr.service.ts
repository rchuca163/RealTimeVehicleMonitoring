import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { AlarmUpdatedEvent } from '../models/alarm-updated-event';

@Injectable({
  providedIn: 'root',
})
export class MonitoringSignalRService {
  private readonly hubConnection: HubConnection;

  constructor() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('https://localhost:7135/hubs/monitoring')
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build();
  }

  async start(): Promise<void> {
    if (this.hubConnection.state !== 'Disconnected') {
      return;
    }

    try {
      await this.hubConnection.start();
      console.log('SignalR connected');
    } catch (error) {
      console.error('SignalR connection error:', error);

      setTimeout(() => {
        this.start();
      }, 5000);
    }
  }

  onReconnected(handler: () => void): void {
    this.hubConnection.onreconnected(() => {
      console.log('SignalR reconnected');
      handler();
    });
  }

  async joinVehicleGroup(vehicleId: number): Promise<void> {
    await this.hubConnection.invoke('JoinVehicleGroup', vehicleId);
  }

  async leaveVehicleGroup(vehicleId: number): Promise<void> {
    await this.hubConnection.invoke('LeaveVehicleGroup', vehicleId);
  }

  onTestMessage(handler: (message: string) => void): void {
    this.hubConnection.on('TestMessage', handler);
  }

  onPositionUpdated(handler: (event: PositionUpdatedEvent) => void): void {
    this.hubConnection.on('PositionUpdated', handler);
  }

  onAlarmUpdated(callback: (event: AlarmUpdatedEvent) => void): void {
    this.hubConnection.on('AlarmUpdated', callback);
  }
}

export interface PositionUpdatedEvent {
  positionId: number;
  vehicleId: number;
  latitude: number;
  longitude: number;
  speed: number;
}