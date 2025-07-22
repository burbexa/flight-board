import { configureStore } from '@reduxjs/toolkit';
import filtersReducer from './filtersSlice';

export const store = configureStore({
  reducer: {
    filters: filtersReducer,
    // Add more slices here
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch; 