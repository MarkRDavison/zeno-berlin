import { useAuth } from "../util/Auth/AuthContext";

export const Home: React.FC = () => {
    const { user } = useAuth();

    return (
        <div>
            {JSON.stringify(user, null, 2)}
        </div>
    );
}