

export default class OrderDto{
    public RequestedAmount : number;
    public ProductCrawlType : string;

    constructor() {
        this.RequestedAmount = 0;
        this.ProductCrawlType = "";
    }
}