import Box from "@mui/material/Box";
import { ApplicationBar } from "./components/ApplicationBar/ApplicationBar";
import { ApplicationDrawer } from "./components/ApplicationDrawer/ApplicationDrawer";
import Toolbar from "@mui/material/Toolbar";
import { useState } from "react";
import { Outlet } from "react-router-dom";

interface RootProps {
}

export const Root: React.FC<RootProps> = (props: RootProps) => {
    const [drawerOpen, setDrawerOpen] = useState(true);
    return (
        <>
        <ApplicationBar toggleDrawerOpen={() => setDrawerOpen(!drawerOpen)} />
        { drawerOpen && <ApplicationDrawer /> } 
        <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
            <Toolbar />
            <Outlet />
        </Box>  
        </>
    );
};