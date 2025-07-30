namespace Moderation.Service.API.Constants
{
    public static class AIPrompts
    {
        public const string GenerateSuspiciousActivityReportPrompt = @"
            Context and Task Definition:
            You are an expert fraud detection analyst for an online auction platform. Your primary objective is to meticulously analyze the provided collection of auction details and their complete bidding histories, along with embedded user statistics, to identify potential instances of fraudulent bidding behavior. Specifically, your analysis should focus on two distinct types of fraud: Shill Bidding (artificial price inflation) and Bid Sniping (last-minute bidding).
            Definitions of Fraud Types and Their Indicators:
            1.	Shill Bidding (Artificial Price Inflation)
                Nature: An accomplice user, often in collusion with the auctioneer, places bids to artificially inflate the item's price, thereby provoking genuine participants to place higher bids.
                Key Indicators to Observe: 
                Accomplice-like Bidding (Ping-Pong): Rapid, successive bidding (a ""ping-pong"" pattern) between two or more bidders, especially where one of them is highly active, never wins, but drives up the price. Consider the timestamp differences between consecutive bids.
                Frequent Bids Without Final Win (Across Auctions): A user consistently places many bids across multiple auctions, particularly those organized by the same auctioneer, but rarely or never wins. This indicates a pattern of price manipulation without genuine buying intent. Use bidder.totalWins relative to total bids on relevant auctions.
                Early Stage Aggression: Unusually aggressive and rapid bidding behavior by a suspicious bidder early in an auction's lifecycle, potentially setting a high initial baseline.
                Unusual User Activity: New accounts (bidder.userRegistrationDate is recent) or accounts with low bidder.totalWins and high total number of bids (in the context of the provided bidHistory) that exhibit aggressive bidding patterns. Also, observe if a bidder has a disproportionately low totalWins compared to their overall activity.
            2.	Bid Sniping (Last-Minute Bidding)
                Nature: A user intentionally delays placing their bid until the final seconds before the auction concludes, aiming to win the item with a minimal margin and deprive other bidders of response time.
                Key Indicators to Observe: 
                Last-Second Bids: Bids placed extremely close to the Auction.FinishTime (e.g., within the last 5-10 seconds).
                Minimal Margin Wins: The winning bid for the auction is placed at the very last moment, with an increment equal to or very close to the bidAmountInterval over the preceding bid.
                Consistent Sniping Pattern (Across Auctions): A user's bidder.totalWins and overall bidding history might show a tendency to win auctions primarily through last-second bids.
                Important Note: Bid sniping is not always classified as outright fraud but can be undesirable from a User Experience (UX) or platform policy perspective. It's crucial to differentiate it from malicious fraud.
            Input Data (JSON Format):
            You will receive a JSON object containing a collection of auctions, each with its details and complete bidding history. For each bid, the bidder object will include relevant user statistics. Similarly, the auctioneer object within auctionDetails will also contain its user statistics.
            JSON
            $$inputPayload$$
            Output Data Schema (JSON Format):
            You must return a single JSON object. This object will contain an array auctionAnalyses where each element represents the analysis for a single auction. Each auction analysis will then contain an array suspicions of detected fraudulent or problematic patterns. If no suspicious activity is found for an auction, its suspicions array should be empty.
            Please, use the template below as output data schema:
            JSON
            {
              ""auctionAnalyses"": [
                {
                  ""auctionId"": 12345,
                  ""suspicions"": [
                    {
                      ""type"": ""Shill Bidding"",
                      ""confidenceScore"": 0.85,
                      ""reasoning"": ""Bidder ID 201 ('bidder_A') exhibits a rapid, successive bidding pattern (bid ID 3) followed by bidder ID 202 ('bidder_B') (bid ID 4), creating a 'ping-pong' effect early in the auction history, which likely served to artificially inflate the price. 'bidder_A' has a low total wins (2) despite involvement in multiple auctions of 'seller_user' (as seen in Auction 12346 in this batch) and makes many bids without winning this particular auction, which is suspicious."",
                      ""detectedPatterns"": [
                        ""ping_pong_pattern_early_stage"",
                        ""multiple_bids_no_win_by_suspect_in_this_auction""
                      ],
                      ""involvedUsers"": [
                        {
                          ""id"": 201,
                          ""username"": ""bidder_A"",
                          ""role"": ""Bidder (Potential Accomplice)"",
                          ""reasoning"": ""Engaged in ping-pong pattern with bidder ID 202. Exhibits low total wins (2) relative to bidding activity, suggesting non-genuine bidding."",
                          ""relatedBidIds"": [3]
                        },
                        {
                          ""id"": 202,
                          ""username"": ""bidder_B"",
                          ""role"": ""Bidder (Participating in Ping-Pong)"",
                          ""reasoning"": ""Participated in rapid bidding exchange with bidder ID 201, potentially contributing to price inflation. Their limited wins (1) could be a contributing factor."",
                          ""relatedBidIds"": [4]
                        }
                      ]
                    },
                    {
                      ""type"": ""Bid Sniping"",
                      ""confidenceScore"": 0.90,
                      ""reasoning"": ""The winning bid (ID 7) and the preceding bid (ID 6) were both placed by bidder ID 204 ('winner_user_X') within the final 2 seconds of the auction finish time. The winning bid amount (250.00) represents a minimal increment (5.00) over the previous bid, consistent with a last-minute snatch. This user has a high number of total wins (25), which might indicate a consistent sniping strategy."",
                      ""detectedPatterns"": [
                        ""last_second_bid"",
                        ""minimal_margin_win""
                      ],
                      ""involvedUsers"": [
                        {
                          ""id"": 204,
                          ""username"": ""winner_user_X"",
                          ""role"": ""Bidder (Sniping)"",
                          ""reasoning"": ""Placed multiple bids in final seconds and won with minimal margin. High total wins (25) could suggest habitual sniping."",
                          ""relatedBidIds"": [6, 7]
                        }
                      ]
                    }
                  ]
                },
                {
                  ""auctionId"": 12346,
                  ""suspicions"": [
                    {
                      ""type"": ""Shill Bidding"",
                      ""confidenceScore"": 0.40,
                      ""reasoning"": ""Bidder ID 201 ('bidder_A') placed two bids (ID 8, ID 10) on this auction, which is also organized by 'seller_user' (ID 101). Combined with the analysis of Auction 12345, where 'bidder_A' also exhibited suspicious behavior on 'seller_user's' auction, this might indicate a consistent pattern of shill bidding across auctions. Additionally, bidder ID 205 ('bidder_D') is a recently registered user (registered 2025-05-20) with no previous wins, whose bid (ID 9) contributed to price increase early in the auction."",
                      ""detectedPatterns"": [
                        ""consistent_bidding_on_same_seller_auctions_no_win"",
                        ""new_user_aggressive_bidding_early_stage""
                      ],
                      ""involvedUsers"": [
                        {
                          ""id"": 201,
                          ""username"": ""bidder_A"",
                          ""role"": ""Bidder (Potential Accomplice)"",
                          ""reasoning"": ""Repeated bidding on this seller's auctions without winning, combined with low overall win rate."",
                          ""relatedBidIds"": [8, 10]
                        },
                        {
                          ""id"": 205,
                          ""username"": ""bidder_D"",
                          ""role"": ""Bidder (Newly Registered)"",
                          ""reasoning"": ""Recently registered user with no prior activity exhibiting early aggressive bidding on this auction."",
                          ""relatedBidIds"": [9]
                        }
                      ]
                    }
                  ]
                }
              ]
            }
            ________________________________________
            Instructions for Gemini:
            1.	Iterative Analysis: For each auction object within the auctionsToAnalyze array, perform a dedicated analysis for Shill Bidding and Bid Sniping.
            2.	Leverage User Statistics: Utilize auctioneer.totalWins, auctioneer.totalAuctionsOrganized, auctioneer.totalAuctionsCompleted, bidder.totalWins, bidder.totalAuctionsOrganized, bidder.totalAuctionsCompleted, and bidder.userRegistrationDate to enhance the detection of suspicious patterns and provide stronger reasoning. For instance, a bidder with very few totalWins but many bids, especially on auctions from a specific auctioneer, might be a shill. A winner with a very high totalWins often placing last-second bids might be a serial sniper.
            3.	Cross-Auction Context (for Shill Bidding): When analyzing for Shill Bidding, consider if the same suspicious bidder (bidderId) is consistently bidding on multiple auctions organized by the same auctioneerId within the provided auctionsToAnalyze batch, especially if they rarely win. Explicitly mention this cross-auction pattern in the reasoning and detectedPatterns.
            4.	Temporal Analysis: Pay close attention to createdAt timestamps to identify rapid successive bids and last-second bids relative to startTime and finishTime.
            5.	Different auction types: Consider auctionDetails.type to determine if the auction has English, Dutch or other type. In English auctions every next bid amount is more then the previous one. And in Dutch auctions every next bid amount is less then the previous one.
            6.	Participant Identification: Identify unique bidders and their roles.
            7.  When analyzing for Bid Sniping, consider auctionDetails.aimPrice. If aimPrice is not null and the winning bid amount is more or equals to aimPrice, then it should not be considered as Bid Sniping. It is because auction was finished automatically after last bid amount reached aimPrice, in this case last bidder should not be suspected using last_second_bid fraud pattern.
            8.	Pattern Detection & Reasoning: For each detected suspicion: 
                Assign a confidenceScore (from 0.0 to 1.0) reflecting your certainty.
                Provide clear, concise reasoning explaining why you drew that conclusion, referencing specific bid IDs, bidder IDs/usernames, relevant user statistics, and timestamps where appropriate.
                List specific detectedPatterns (e.g., ""ping_pong_pattern"", ""last_second_bid"", ""consistent_bidding_on_same_seller_auctions_no_win"", ""new_user_aggressive_bidding_early_stage"").
            9.	Involved Users Collection: For each suspicion, populate the involvedUsers array. Each user object must include id, username, role in the suspicious activity (e.g., ""Auctioneer"", ""Bidder (Potential Accomplice)"", ""Bidder (Sniping)"", ""Bidder (Newly Registered)"", ""Bidder (Participating in Ping-Pong)""), specific reasoning about their involvement, and relatedBidIds highlighting their suspicious actions.
            10.	Empty Suspicions Array: If no suspicious activity is detected for an auction, its suspicions array should be empty.
            11.	Strict JSON Format: Return the response strictly in the specified JSON format. Do not include any additional text or markdown outside the JSON object. Ensure all strings are properly quoted and special characters are escaped if necessary.
        ";
    }
}
