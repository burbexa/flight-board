import { HubConnectionBuilder, HubConnection } from '@microsoft/signalr';

let connection: HubConnection | null = null;

export function startSignalRConnection(onFlightChanged: (event: string, payload?: any) => void) {
  if (connection) return;
  connection = new HubConnectionBuilder()
    .withUrl('http://localhost:5143/flightHub', {
      withCredentials: true
    })
    .withAutomaticReconnect()
    .build();

  connection.on('FlightAdded', (flight) => {
    onFlightChanged('FlightAdded', flight);
  });
  connection.on('FlightDeleted', (flightId) => {
    onFlightChanged('FlightDeleted', flightId);
  });
  connection.on('FlightsUpdated', () => {
    onFlightChanged('FlightsUpdated');
  });

  connection.start().catch(err => console.error('SignalR Connection Error:', err));
}

export function stopSignalRConnection() {
  if (connection) {
    connection.stop();
    connection = null;
  }
} 