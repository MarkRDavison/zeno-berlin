import Box from "@mui/material/Box";
import { ApplicationBar } from "./components/ApplicationBar/ApplicationBar";
import { ApplicationDrawer } from "./components/ApplicationDrawer/ApplicationDrawer";
import { Home } from "./pages/Home";
import { useAuth } from "./util/Auth/AuthContext";
import { RouterProvider, createBrowserRouter } from 'react-router-dom';
import Toolbar from "@mui/material/Toolbar";
import { useState } from "react";


const router = createBrowserRouter([
    {
      path: '/',
      element: <Home />
    }
  ]);

export const AppWithRouterAccess: React.FC = () => {
    const { isLoggedIn, isLoggingIn, login } = useAuth();
    const [drawerOpen, setDrawerOpen] = useState(true);

    if (!isLoggedIn && isLoggingIn){            
        return <div className='entire-app-loading'></div>;
    }
    
    if (isLoggedIn && !isLoggingIn) {
        return (
            <Box sx={{ display: 'flex' }}>
                <ApplicationBar toggleDrawerOpen={() => setDrawerOpen(!drawerOpen)} />
                { drawerOpen && <ApplicationDrawer /> } 
                <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
                    <Toolbar />
                    <RouterProvider router={router} />
                </Box>            
            </Box>
        );
    }
    else {
        login();
        return <div className='entire-app-loading'></div>;
    }
}