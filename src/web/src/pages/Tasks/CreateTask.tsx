import { useAuth } from "../../util/Auth/AuthContext";

export const CreateTask: React.FC = () => {
    const { user } = useAuth();

    return (
        <div>
            <h3>Create task</h3>
            {JSON.stringify(user, null, 2)}
        </div>
    );
}