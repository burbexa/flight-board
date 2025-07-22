import React from 'react';
import { FlightStatus } from '../types/flight';
import styles from '../styles/FlightFilters.module.css';
import TextField from '@mui/material/TextField';
import MenuItem from '@mui/material/MenuItem';
import Button from '@mui/material/Button';

interface FlightFiltersProps {
  status: FlightStatus | '';
  destination: string;
  onStatusChange: (status: FlightStatus | '') => void;
  onDestinationChange: (destination: string) => void;
  onClear: () => void;
}

const statusOptions: (FlightStatus | '')[] = ['', 'Scheduled', 'Boarding', 'Departed', 'Landed'];

const FlightFilters: React.FC<FlightFiltersProps> = ({
  status,
  destination,
  onStatusChange,
  onDestinationChange,
  onClear,
}) => {
  return (
    <div className={styles.filters}>
      <TextField
        select
        label="Status"
        value={status}
        onChange={e => onStatusChange(e.target.value as FlightStatus | '')}
        size="small"
        style={{ minWidth: 140 }}
      >
        {statusOptions.map(option => (
          <MenuItem key={option} value={option}>
            {option || 'All Statuses'}
          </MenuItem>
        ))}
      </TextField>
      <TextField
        label="Destination"
        value={destination}
        onChange={e => onDestinationChange(e.target.value)}
        size="small"
        style={{ minWidth: 180 }}
      />
      <Button variant="outlined" color="secondary" onClick={onClear}>
        Clear Filters
      </Button>
    </div>
  );
};

export default FlightFilters; 