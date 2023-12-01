import { createSlice, PayloadAction } from '@reduxjs/toolkit'
import { RootState } from '../../store/store'

interface UserContextState {
    isAdmin: boolean
}

const initialState: UserContextState = {
    isAdmin: false
}

export const userContextSlice = createSlice({
    name: 'userContext',
    initialState,
    reducers: {
        setAdmin: (state, action: PayloadAction<boolean>) => {
            state.isAdmin = action.payload
        }
    }
})

export const {
    setAdmin
} = userContextSlice.actions;

export const selectUserContext = (state: RootState): UserContextState => state.userContext;
export const selectAdminState = (state: RootState): boolean => state.userContext.isAdmin;

export default userContextSlice.reducer;