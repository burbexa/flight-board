import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { FlightStatus } from '../types/flight';

interface FiltersState {
  status: FlightStatus | '';
  destination: string;
}

const initialState: FiltersState = {
  status: '',
  destination: '',
};

const filtersSlice = createSlice({
  name: 'filters',
  initialState,
  reducers: {
    setStatus(state, action: PayloadAction<FlightStatus | ''>) {
      state.status = action.payload;
    },
    setDestination(state, action: PayloadAction<string>) {
      state.destination = action.payload;
    },
    clearFilters(state) {
      state.status = '';
      state.destination = '';
    },
  },
});

export const { setStatus, setDestination, clearFilters } = filtersSlice.actions;
export default filtersSlice.reducer; 