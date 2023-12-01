export interface BaseResponse {
    errors: string[]
    warnings: string[]
    success: boolean
}

export interface Response<T> extends BaseResponse{
    content: T | null
}