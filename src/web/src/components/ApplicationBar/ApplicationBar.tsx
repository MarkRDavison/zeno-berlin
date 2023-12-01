import AppBar from "@mui/material/AppBar";
import Box from "@mui/material/Box";
import IconButton from "@mui/material/IconButton";
import Toolbar from "@mui/material/Toolbar";
import { Menu as MenuIcon, AccountCircle as AccountCircleIcon } from '@mui/icons-material';
import { Menu, MenuItem, Typography } from "@mui/material";
import { useState } from "react";
import { useAuth } from "../../util/Auth/AuthContext";

export interface ApplicationBarProps {
    toggleDrawerOpen: () => void
}

export const ApplicationBar: React.FC<ApplicationBarProps> = (
    props: ApplicationBarProps
) => {
    const { logout } = useAuth();
    const { toggleDrawerOpen } = props;
    
    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);

    const handleMenu = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    };

    const handleClose = () => {
        setAnchorEl(null);
    };

    return (
        <Box sx={{ flexGrow: 1 }}>
            <AppBar position="fixed"  sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
                <Toolbar>
                    <IconButton 
                        onClick={toggleDrawerOpen}
                        size="large"
                        edge="start"
                        color="inherit"
                        aria-label="menu"
                        sx={{ marginRight: 2 }}>
                        <MenuIcon />
                    </IconButton>
                    <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
                        Berlin
                    </Typography> 
                    <IconButton
                        size="large"
                        aria-label="account of current user"
                        aria-controls="menu-appbar"
                        aria-haspopup="true"
                        onClick={handleMenu}
                        color="inherit"
                    >
                        <AccountCircleIcon />
                    </IconButton>
                    <Menu
                        id="menu-appbar"
                        anchorEl={anchorEl}
                        anchorOrigin={{
                            vertical: 'top',
                            horizontal: 'right',
                        }}
                        keepMounted
                        transformOrigin={{
                            vertical: 'top',
                            horizontal: 'right',
                        }}
                        open={Boolean(anchorEl)}
                        onClose={handleClose}
                    >
                        <MenuItem onClick={handleClose}>Profile</MenuItem>
                        <MenuItem onClick={logout}>Logout</MenuItem>
                    </Menu>
                </Toolbar>
            </AppBar>
        </Box>
    );
};