import './App.css'
import { AppWithRouterAccess } from './AppWithRouterAccess';
import { AuthProvider } from './util/Auth/AuthContext'

export const App: React.FC = () => {


  return (
    <AuthProvider>
      <AppWithRouterAccess />
    </AuthProvider>
  )
}
