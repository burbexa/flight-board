export type FlightStatus =
  | 'Scheduled'
  | 'Boarding'
  | 'Departed'
  | 'Landed';

export interface Flight {
  id: string;
  flightNumber: string;
  destination: string;
  departureTime: string; // ISO string
  gate: string;
  status: FlightStatus;
}

export interface FlightFormValues {
  flightNumber: string;
  destination: string;
  departureTime: string;
  gate: string;
} 