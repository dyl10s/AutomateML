export class ReturnResult<T>{
    public success: boolean;
    public errorMessage: string;
    public exception: any;
    public item: T;
}