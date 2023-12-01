import { setAdmin } from "../features/userContext/userContextSlice";
import { AppDispatch } from "../store/store";
import config from "../util/config";
import { StartupQueryResponse } from "./scenarios/queries/StartupQuery";

export const client = {
    fetchStartup: async (dispatch: AppDispatch): Promise<void> => {
        const response = await fetch(config.bff_base_uri + '/api/startup-query', {
            credentials: "include"
        });

        if (response.ok) {
            var responseObject: StartupQueryResponse = await response.json()
            if (responseObject.success) {
                dispatch(setAdmin(responseObject.admin));
            }
        }
    }
}