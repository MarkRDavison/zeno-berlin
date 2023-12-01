import { Provider } from 'react-redux';
import './App.css'
import { AppWithRouterAccess } from './AppWithRouterAccess';
import { AuthProvider } from './util/Auth/AuthContext'
import store from './store/store';

export const App: React.FC = () => {


  return (
    <Provider store={store}>
      <AuthProvider>
          <AppWithRouterAccess />
      </AuthProvider>
    </Provider>
  )
}
