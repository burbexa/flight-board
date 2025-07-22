import { Flight, FlightFormValues, FlightStatus } from '../types/flight';

const API_BASE = 'http://localhost:5143/api/flights';

export async function getFlights(): Promise<Flight[]> {
  const res = await fetch(API_BASE);
  if (!res.ok) throw new Error('Failed to fetch flights');
  return res.json();
}

export async function addFlight(data: FlightFormValues): Promise<Flight> {
  const res = await fetch(API_BASE, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  });
  if (!res.ok) throw new Error('Failed to add flight');
  return res.json();
}

export async function deleteFlight(id: string): Promise<void> {
  const res = await fetch(`${API_BASE}/${id}`, {
    method: 'DELETE',
  });
  if (!res.ok) throw new Error('Failed to delete flight');
}

export async function searchFlights(
  status?: FlightStatus,
  destination?: string
): Promise<Flight[]> {
  const params = new URLSearchParams();
  if (status) params.append('status', status);
  if (destination) params.append('destination', destination);
  const url = `http://localhost:5143/api/flights/search?${params.toString()}`;
  const res = await fetch(url);
  if (!res.ok) throw new Error('Failed to search flights');
  return res.json();
} 