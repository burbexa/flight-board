import React, { useEffect, useRef, useState } from 'react';
import { Provider, useSelector, useDispatch } from 'react-redux';
import { QueryClient, QueryClientProvider, useQueryClient } from '@tanstack/react-query';
import { SnackbarProvider, useSnackbar } from 'notistack';
import { store, RootState } from './features/store';
import FlightTable from './components/FlightTable';
import FlightForm from './components/FlightForm';
import FlightFilters from './components/FlightFilters';
import { useFlights, useAddFlight, useDeleteFlight, useSearchFlights } from './api/flightHooks';
import { startSignalRConnection, stopSignalRConnection } from './api/signalRClient';
import { setStatus, setDestination, clearFilters } from './features/filtersSlice';
import { FlightFormValues } from './types/flight';
import './App.css';

const queryClient = new QueryClient();

const AppContent: React.FC = () => {
  const dispatch = useDispatch();
  const filters = useSelector((state: RootState) => state.filters);
  const queryClient = useQueryClient();
  const { enqueueSnackbar } = useSnackbar();
  const [newFlightId, setNewFlightId] = useState<string | null>(null);
  const newFlightTimeout = useRef<NodeJS.Timeout | null>(null);

  // Always call hooks in the same order
  const flightsQuery = useFlights();
  const searchFlightsQuery = useSearchFlights(
    filters.status === '' ? undefined : filters.status,
    filters.destination
  );
  const addFlight = useAddFlight();
  const deleteFlight = useDeleteFlight();

  // Determine if any filter is active
  const isFiltering = filters.status !== '' || filters.destination.trim() !== '';
  const flightsData = isFiltering ? searchFlightsQuery.data : flightsQuery.data;
  const flightsLoading = isFiltering ? searchFlightsQuery.isLoading : flightsQuery.isLoading;
  const flightsError = isFiltering ? searchFlightsQuery.isError : flightsQuery.isError;

  const handleAddFlight = (values: FlightFormValues) => {
    addFlight.mutate(values, {
      onSuccess: (flight) => {
        setNewFlightId(flight.id);
        if (newFlightTimeout.current) clearTimeout(newFlightTimeout.current);
        newFlightTimeout.current = setTimeout(() => setNewFlightId(null), 2000);
        if (isFiltering) searchFlightsQuery.refetch();
      },
    });
  };

  const handleDeleteFlight = (id: string) => {
    deleteFlight.mutate(id, {
      onSuccess: () => {
        if (isFiltering) searchFlightsQuery.refetch();
      },
    });
  };

  const handleStatusChange = (status: string) => {
    dispatch(setStatus(status as any));
  };

  const handleDestinationChange = (dest: string) => {
    dispatch(setDestination(dest));
  };

  const handleClear = () => {
    dispatch(clearFilters());
  };

  useEffect(() => {
    startSignalRConnection((event, payload) => {
      if (event === 'FlightAdded' && payload) {
        enqueueSnackbar(
          `Flight added: ${payload.flightNumber} to ${payload.destination} at ${new Date(payload.departureTime).toLocaleString()}`,
          { variant: 'success' }
        );
      } else if (event === 'FlightDeleted') {
        enqueueSnackbar(
          `A flight was deleted${payload ? ` (ID: ${payload})` : ''}.`,
          { variant: 'info' }
        );
      }
      setTimeout(() => {
        queryClient.invalidateQueries({ queryKey: ['flights'], exact: false });
      }, 100);
    });
    return () => {
      stopSignalRConnection();
    };
  }, [queryClient, enqueueSnackbar]);

  return (
    <div className="App">
      <h1>Flight Board</h1>
      <FlightForm
        onSubmit={handleAddFlight}
        existingFlights={flightsQuery.data || []}
        loading={addFlight.status === 'pending'}
      />
      <FlightFilters
        status={filters.status}
        destination={filters.destination}
        onStatusChange={handleStatusChange}
        onDestinationChange={handleDestinationChange}
        onClear={handleClear}
      />
      {flightsLoading ? (
        <p>Loading flights...</p>
      ) : flightsError ? (
        <p>Error loading flights.</p>
      ) : (
        <FlightTable flights={flightsData || []} onDelete={handleDeleteFlight} newFlightId={newFlightId || undefined} />
      )}
    </div>
  );
};

function App() {
  return (
    <Provider store={store}>
      <QueryClientProvider client={queryClient}>
        <SnackbarProvider maxSnack={3} anchorOrigin={{ vertical: 'top', horizontal: 'center' }}>
          <AppContent />
        </SnackbarProvider>
      </QueryClientProvider>
    </Provider>
  );
}

export default App;
