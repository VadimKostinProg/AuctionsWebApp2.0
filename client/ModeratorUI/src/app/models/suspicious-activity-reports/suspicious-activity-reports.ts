export class SuspiciousActivityReport {
  public reportId!: string;
  public auctionAnalyses!: AuctionAnalysis[];
}

export class AuctionAnalysis {
  public auctionAnalysisId!: string;
  public auctionId!: number;
  public overallAnalysisSummary?: string;
  public suspicions!: Suspicion[];
}

export class Suspicion {
  public type!: 'Shill Bidding' | 'Bid Sniping' | string;
  public isLikely?: boolean;
  public isDetected?: boolean;
  public isPotentiallyProblematic?: boolean;
  public confidenceScore!: number;
  public reasoning!: string;
  public detectedPatterns!: string[];
  public involvedUsers!: InvolvedUser[];
}

export class InvolvedUser {
  public userId!: number;
  public username!: string;
  public role!: string;
  public reasoning!: string;
  public relatedBidIds!: number[];
}
