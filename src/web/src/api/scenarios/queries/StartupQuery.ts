import { BaseResponse } from "../Response";

export interface StartupQueryRequest {

}

export interface StartupQueryResponse extends BaseResponse {
    admin: boolean
}