import { FlightStatus } from '../types/flight';

/**
 * Calculates the status of a flight based on the current time and departure time.
 *
 * Boarding: From 30 minutes before departure until the departure time.
 * Departed: From the departure time until 60 minutes after.
 * Landed: More than 60 minutes after departure time.
 * Scheduled: More than 30 minutes before departure time.
 */
export function calculateFlightStatus(departureTime: string, now: Date = new Date()): FlightStatus {
  const dep = new Date(departureTime).getTime();
  const current = now.getTime();
  const diffMinutes = (dep - current) / 60000;

  if (diffMinutes > 30) {
    return 'Scheduled';
  } else if (diffMinutes <= 30 && diffMinutes > 0) {
    return 'Boarding';
  } else if (diffMinutes <= 0 && diffMinutes > -60) {
    return 'Departed';
  } else {
    return 'Landed';
  }
} 