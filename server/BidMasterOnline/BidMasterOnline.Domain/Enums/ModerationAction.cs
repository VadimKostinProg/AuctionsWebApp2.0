namespace BidMasterOnline.Domain.Enums
{
    public enum ModerationAction
    {
        BlockingUser,
        UnblockingUser,
        ApprovingAuctionRequest,
        DecliningAuctionRequest,
        CancelingAuction,
        DeletingComment,
        PickingUpTechnicalSupportRequest,
        CompletingTechnicalSupportRequest,
        PickingUpComplaint,
        CompletingComplaint
    }
}
