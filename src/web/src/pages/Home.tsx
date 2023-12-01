import { useAuth } from "../util/Auth/AuthContext";
import { useAppSelector } from "../store/hooks";
import { selectAdminState } from "../features/userContext/userContextSlice";

export const Home: React.FC = () => {
    const { user } = useAuth();
    const isAdmin = useAppSelector(selectAdminState)

    return (
        <div>
            {JSON.stringify(user, null, 2)}
            <br />
            {isAdmin ? 'Is admin' : 'Is not admin'}
        </div>
    );
}