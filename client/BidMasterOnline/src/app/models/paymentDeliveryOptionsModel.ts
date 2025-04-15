export class PaymentDeliveryOptionsModel {
    public id: string;
    public auctionId: string;
    public winnerId: string;
    public winner: string;
    public arePaymentOptionsSet: boolean;
    public paymentOptionsSetDateTime: string;
    public iban: string;
    public areDeliveryOptionsSet: boolean;
    public deliveryOptionsSetDateTime: string;
    public country: string;
    public city: string;
    public zipCode: string;
    public isPaymentConfirmed: boolean;
    public paymentConfirmationDateTime: string;
    public isDeliveryConfirmed: boolean;
    public deliveryConfirmationDateTime: string;
    public wayBill: string;
}