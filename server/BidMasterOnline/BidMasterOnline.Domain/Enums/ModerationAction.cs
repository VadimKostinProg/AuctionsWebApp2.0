namespace BidMasterOnline.Domain.Enums
{
    public enum ModerationAction
    {
        BlockingUser,
        UnblockingUser,
        ApprovingAuctionRequest,
        DecliningAuctionRequest,
        CancelingAuction,
        RecoveringAuction,
        DeletingComment,
        PickingUpTechnicalSupportRequest,
        CompletingTechnicalSupportRequest,
        PickingUpComplaint,
        CompletingComplaint
    }
}
