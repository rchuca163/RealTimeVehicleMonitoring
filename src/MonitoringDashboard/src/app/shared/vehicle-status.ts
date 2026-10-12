export const VEHICLE_STATUS = {
  normal: {
    label: 'En tránsito',
    color: '#00e68a',
  },
  speeding: {
    label: 'Exceso de velocidad',
    color: '#ffb020',
  },
  panic: {
    label: 'Alarma de pánico',
    color: '#ff4757',
  },
  stopped: {
    label: 'Detenido',
    color: '#4da6ff',
  },
} as const;