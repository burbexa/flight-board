import { FlightFormValues, Flight } from '../types/flight';

export interface FlightValidationErrors {
  flightNumber?: string;
  destination?: string;
  departureTime?: string;
  gate?: string;
}

export function validateFlightForm(
  values: FlightFormValues,
  existingFlights: Flight[] = []
): FlightValidationErrors {
  const errors: FlightValidationErrors = {};

  if (!values.flightNumber) {
    errors.flightNumber = 'Flight number is required';
  } else if (existingFlights.some(f => f.flightNumber === values.flightNumber)) {
    errors.flightNumber = 'Flight number must be unique';
  }

  if (!values.destination) {
    errors.destination = 'Destination is required';
  }

  if (!values.gate) {
    errors.gate = 'Gate is required';
  }

  if (!values.departureTime) {
    errors.departureTime = 'Departure time is required';
  } else if (new Date(values.departureTime) <= new Date()) {
    errors.departureTime = 'Departure time must be in the future';
  }

  return errors;
} 