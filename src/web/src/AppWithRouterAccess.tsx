import Box from "@mui/material/Box";
import { UserDashboard } from "./pages/User/UserDashboard";
import { useAuth } from "./util/Auth/AuthContext";
import { RouterProvider, createBrowserRouter } from 'react-router-dom';
import { Root } from "./Root";
import { AdminUsers } from "./pages/Admin/AdminUsers";
import { AdminDashboard } from "./pages/Admin/AdminDashboard";
import { AdminShares } from "./pages/Admin/AdminShares";
import { AdminJobs } from "./pages/Admin/AdminJobs";
import { UserJobs } from "./pages/User/UserJobs";


const router = createBrowserRouter([
    {
      path: '/',
      element: <Root />,
      children: [
        {
            path: '/',
            element: <UserDashboard />
        },
        {
            path: '/user-jobs',
            element: <UserJobs />
        },
        {
            path: '/admin-users',
            element: <AdminUsers />
        },
        {
            path: '/admin-shares',
            element: <AdminShares />
        },
        {
            path: '/admin-dashboard',
            element: <AdminDashboard />
        },
        {
            path: '/admin-Jobs',
            element: <AdminJobs />
        }
      ]
    }
  ]);

export const AppWithRouterAccess: React.FC = () => {
    const { isLoggedIn, isLoggingIn, login } = useAuth();

    if (!isLoggedIn && isLoggingIn){            
        return <div className='entire-app-loading'></div>;
    }
    
    if (isLoggedIn && !isLoggingIn) {
        return (
            <Box sx={{ display: 'flex' }}>
                <RouterProvider router={router} />    
            </Box>
        );
    }
    else {
        login();
        return <div className='entire-app-loading'></div>;
    }
}