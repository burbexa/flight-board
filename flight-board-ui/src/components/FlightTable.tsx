import React, { useState } from 'react';
import { Flight } from '../types/flight';
import styles from '../styles/FlightTable.module.css';
import DeleteIcon from '@mui/icons-material/Delete';

interface FlightTableProps {
  flights: Flight[];
  onDelete: (id: string) => void;
  newFlightId?: string;
}

const statusClass = (status: string) => {
  switch (status) {
    case 'Scheduled': return styles.statusScheduled;
    case 'Boarding': return styles.statusBoarding;
    case 'Departed': return styles.statusDeparted;
    case 'Landed': return styles.statusLanded;
    default: return '';
  }
};

const FlightTable: React.FC<FlightTableProps> = ({ flights, onDelete, newFlightId }) => {
  const [hoveredRow, setHoveredRow] = useState<string | null>(null);

  return (
    <table className={styles.table}>
      <thead>
        <tr>
          <th>Flight Number</th>
          <th>Destination</th>
          <th>Departure Time</th>
          <th>Gate</th>
          <th>Status</th>
          <th></th>
        </tr>
      </thead>
      <tbody>
        {flights.map(flight => {
          const isHovered = hoveredRow === flight.id;
          const isNew = newFlightId === flight.id;
          const rowClass = [
            isHovered ? styles.rowHover : '',
            isNew ? styles.newFlight : ''
          ].filter(Boolean).join(' ');
          return (
            <tr
              key={flight.id}
              className={rowClass}
              onMouseEnter={() => setHoveredRow(flight.id)}
              onMouseLeave={() => setHoveredRow(null)}
            >
              <td>{flight.flightNumber}</td>
              <td>{flight.destination}</td>
              <td>{new Date(flight.departureTime).toLocaleString()}</td>
              <td>{flight.gate}</td>
              <td>
                <span className={statusClass(flight.status)}>{flight.status}</span>
              </td>
              <td>
                <span className={styles.deleteBtnWrapper}>
                  <button
                    className={styles.deleteBtn}
                    onClick={() => onDelete(flight.id)}
                    aria-label="Delete flight"
                  >
                    <DeleteIcon />
                  </button>
                </span>
              </td>
            </tr>
          );
        })}
      </tbody>
    </table>
  );
};

export default FlightTable; 