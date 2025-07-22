import React, { useState } from 'react';
import { FlightFormValues, Flight } from '../types/flight';
import { validateFlightForm, FlightValidationErrors } from '../utils/flightValidation';
import styles from '../styles/FlightForm.module.css';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';

interface FlightFormProps {
  onSubmit: (values: FlightFormValues) => void;
  existingFlights: Flight[];
  loading?: boolean;
}

const initialForm: FlightFormValues = {
  flightNumber: '',
  destination: '',
  departureTime: '',
  gate: '',
};

const FlightForm: React.FC<FlightFormProps> = ({ onSubmit, existingFlights, loading }) => {
  const [values, setValues] = useState<FlightFormValues>(initialForm);
  const [errors, setErrors] = useState<FlightValidationErrors>({});

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setValues({ ...values, [e.target.name]: e.target.value });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const validation = validateFlightForm(values, existingFlights);
    setErrors(validation);
    if (Object.keys(validation).length === 0) {
      // Convert local datetime-local to UTC ISO string
      const localDate = new Date(values.departureTime);
      const utcISOString = new Date(localDate.getTime() - localDate.getTimezoneOffset() * 60000).toISOString();
      onSubmit({ ...values, departureTime: utcISOString });
      setValues(initialForm);
    }
  };

  return (
    <form className={styles.form} onSubmit={handleSubmit} noValidate>
      <TextField
        label="Flight Number"
        name="flightNumber"
        value={values.flightNumber}
        onChange={handleChange}
        error={!!errors.flightNumber}
        helperText={errors.flightNumber}
        size="small"
        required
      />
      <TextField
        label="Destination"
        name="destination"
        value={values.destination}
        onChange={handleChange}
        error={!!errors.destination}
        helperText={errors.destination}
        size="small"
        required
      />
      <TextField
        label="Gate"
        name="gate"
        value={values.gate}
        onChange={handleChange}
        error={!!errors.gate}
        helperText={errors.gate}
        size="small"
        required
      />
      <TextField
        label="Departure Time"
        name="departureTime"
        type="datetime-local"
        value={values.departureTime}
        onChange={handleChange}
        error={!!errors.departureTime}
        helperText={errors.departureTime}
        size="small"
        required
        InputLabelProps={{ shrink: true }}
      />
      <Button
        type="submit"
        variant="contained"
        color="primary"
        disabled={loading}
      >
        Add Flight
      </Button>
    </form>
  );
};

export default FlightForm; 