export interface TimeSlot {
  id: string;
  dayOfWeek: number;
  startTime: string; // "HH:mm"
  endTime: string;   // "HH:mm"
  isAvailable: boolean;
}

export interface CreateTimeSlotRequest {
  dayOfWeek: number;
  startTime: string;
  endTime: string;
  isAvailable: boolean;
}

export interface UpdateTimeSlotRequest {
  dayOfWeek: number;
  startTime: string;
  endTime: string;
  isAvailable: boolean;
}
