import { Home } from "./pages/Home";
import { CreateTask } from "./pages/Tasks/CreateTask";
import { useAuth } from "./util/Auth/AuthContext";
import { RouterProvider, createBrowserRouter } from 'react-router-dom';

const router = createBrowserRouter([
    {
        path: '/task/create',
        element: <CreateTask />
    },
    {
      path: '/',
      element: <Home />
    }
  ]);

export const AppWithRouterAccess: React.FC = () => {
    const { isLoggedIn, isLoggingIn, login } = useAuth();


    if (!isLoggedIn && isLoggingIn){            
        return <div className='entire-app-loading'></div>;
    }
    
    if (isLoggedIn && !isLoggingIn) {
        return (
            <RouterProvider router={router} />
        );
    }
    else {
        login();
        return <div className='entire-app-loading'></div>;
    }
}