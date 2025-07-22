import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import * as api from './mockFlights';
import { Flight, FlightFormValues, FlightStatus } from '../types/flight';

export function useFlights() {
  return useQuery<Flight[], Error>({
    queryKey: ['flights'],
    queryFn: api.getFlights,
  });
}

export function useAddFlight() {
  const queryClient = useQueryClient();
  return useMutation<Flight, Error, FlightFormValues>({
    mutationFn: api.addFlight,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['flights'], exact: false });
    },
  });
}

export function useDeleteFlight() {
  const queryClient = useQueryClient();
  return useMutation<void, Error, string>({
    mutationFn: api.deleteFlight,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['flights'], exact: false });
    },
  });
}

export function useSearchFlights(status?: FlightStatus, destination?: string) {
  return useQuery<Flight[], Error>({
    queryKey: ['flights', { status, destination }],
    queryFn: () => api.searchFlights(status, destination),
  });
} 