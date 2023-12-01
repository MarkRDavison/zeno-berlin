import Drawer from "@mui/material/Drawer";
import List from "@mui/material/List";
import ListItemButton from "@mui/material/ListItemButton";
import ListItemIcon from "@mui/material/ListItemIcon";
import ListItemText from "@mui/material/ListItemText";
import Toolbar from "@mui/material/Toolbar";
import { ExpandLess, ExpandMore, StarBorder } from '@mui/icons-material';
import Collapse from "@mui/material/Collapse";
import { useState } from "react";
import { Link as ReactRouterLink } from "react-router-dom";
import { useAppSelector } from "../../store/hooks";
import { selectAdminState } from "../../features/userContext/userContextSlice";


const drawerWidth = 240;

export interface ApplicationDrawerProps {

}

export const ApplicationDrawer: React.FC<ApplicationDrawerProps> = (props: ApplicationDrawerProps) => {
    const {  } = props;
    const [open, setOpen] = useState(true);
    const isAdmin = useAppSelector(selectAdminState);

    const handleClick = () => {
      setOpen(!open);
    };

    return (
        <Drawer
            open={true}
            variant="persistent"
            sx={{
                width: drawerWidth,
                flexShrink: 0,
                [`& .MuiDrawer-paper`]: { width: drawerWidth, boxSizing: 'border-box' },
              }}>
            
        <Toolbar />
        <List
            sx={{ width: '100%', maxWidth: 360, bgcolor: 'background.paper' }}
            component="nav"
            aria-labelledby="nested-list-subheader"
          >
          <ListItemButton component={ReactRouterLink} to="/">
            <ListItemIcon>
              <StarBorder />
            </ListItemIcon>
            <ListItemText primary="Dashboard" />
          </ListItemButton>
            <ListItemButton component={ReactRouterLink} to="/user-jobs">
              <ListItemIcon>
                <StarBorder />
              </ListItemIcon>
              <ListItemText primary="Jobs" />
            </ListItemButton>

             {isAdmin && <>

            <ListItemButton onClick={handleClick}>
              <ListItemIcon>
                <StarBorder />
              </ListItemIcon>
              <ListItemText primary="Administration" />
              {open ? <ExpandLess /> : <ExpandMore />}
            </ListItemButton>
            <Collapse in={open} timeout="auto" unmountOnExit>
              <List component="div" disablePadding>
                <ListItemButton sx={{ pl: 4 }} component={ReactRouterLink} to="/admin-users">
                  <ListItemIcon>
                    <StarBorder />
                  </ListItemIcon>
                  <ListItemText primary="Users"/>
                </ListItemButton>
                <ListItemButton sx={{ pl: 4 }} component={ReactRouterLink} to="/admin-shares">
                  <ListItemIcon>
                    <StarBorder />
                  </ListItemIcon>
                  <ListItemText primary="Shares" />
                </ListItemButton>
                <ListItemButton sx={{ pl: 4 }} component={ReactRouterLink} to="/admin-dashboard">
                  <ListItemIcon>
                    <StarBorder />
                  </ListItemIcon>
                  <ListItemText primary="Admin dashboard" />
                </ListItemButton>
                <ListItemButton sx={{ pl: 4 }} component={ReactRouterLink} to="/admin-jobs">
                  <ListItemIcon>
                    <StarBorder />
                  </ListItemIcon>
                  <ListItemText primary="Admin jobs" />
                </ListItemButton>
              </List>
            </Collapse>
            
            </>}
          </List>
        </Drawer>
    );
};