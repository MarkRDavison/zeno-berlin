import { Store, configureStore } from '@reduxjs/toolkit'
import userContextReducer from '../features/userContext/userContextSlice'

const store: Store = configureStore({
    reducer: {
        userContext: userContextReducer
    }
});

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch
export type GetStateType = () => RootState;

export default store;