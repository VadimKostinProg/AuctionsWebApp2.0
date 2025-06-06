import { Component } from '@angular/core';
import { Suspicion, SuspiciousActivityReport } from '../../models/suspicious-activity-reports/suspicious-activity-reports';
import { SuspiciousActivityReportsService } from '../../services/suspicious-activity-reports.service';
import { catchError, finalize, map, of } from 'rxjs';
import { SuspiciousActivityCheckPeriodEnum } from '../../models/suspicious-activity-reports/suspicious-activity-check-period';

@Component({
  selector: 'app-suspicious-activity-reports-list',
  templateUrl: './suspicious-activity-reports-list.component.html',
  styleUrls: ['./suspicious-activity-reports-list.component.scss'],
  standalone: false
})
export class SuspiciousActivityReportsListComponent {
  report: SuspiciousActivityReport | null = null;
  loading: boolean = false;
  errorMessage: string | null = null;
  selectedPeriod: SuspiciousActivityCheckPeriodEnum = SuspiciousActivityCheckPeriodEnum.LastDay;
  periods = [
    {
      label: 'Last day',
      value: SuspiciousActivityCheckPeriodEnum.LastDay
    },
    {
      label: 'Last week',
      value: SuspiciousActivityCheckPeriodEnum.LastWeek
    },
    {
      label: 'Last mongth',
      value: SuspiciousActivityCheckPeriodEnum.LastMongth
    }
  ]

  constructor(private reportService: SuspiciousActivityReportsService) { }

  generateReport(): void {
    this.report = null;
    this.loading = true;
    this.errorMessage = null;

    this.reportService.getSuspiciousActivityReport(this.selectedPeriod).pipe(
      map(serviceResult => {
        if (serviceResult.isSuccessfull) {
          if (!serviceResult.data) {
            return {
              reportId: 'N/A',
              auctionAnalyses: [],
              analysisTimestamp: new Date().toISOString(),
              batchProcessingStatus: 'Completed'
            } as SuspiciousActivityReport;
          }

          return serviceResult.data;
        } else {
          throw new Error('Failed to fetch report.');
        }
      }),
      catchError(err => {
        this.errorMessage = err.error.errors[0] || 'An unknown error occurred while generating report.';
        console.error('Error generating report:', err);
        return of(null);
      }),
      finalize(() => this.loading = false)
    ).subscribe(
      data => {
        this.report = data;
      }
    );
  }

  getConfidenceBadgeClass(confidence: number | undefined): string {
    if (confidence === undefined) return 'bg-secondary';
    if (confidence >= 0.8) return 'bg-danger';
    if (confidence >= 0.5) return 'bg-warning';
    return 'bg-success';
  }

  getConfidenceText(confidence: number | undefined): string {
    if (confidence === undefined) return 'N/A';
    return `${(confidence * 100).toFixed(0)}%`;
  }

  getSuspicionHeaderClass(suspicion: Suspicion): string {
    const confidence = suspicion.confidenceScore;
    if (confidence >= 0.8) return 'bg-danger';
    if (confidence >= 0.5) return 'bg-warning';
    return 'bg-info';
  }

  getSuspicionIcon(type: string): string {
    switch (type) {
      case 'Shill Bidding': return 'fas fa-bullhorn';
      case 'Bid Sniping': return 'fas fa-crosshairs';
      default: return 'fas fa-exclamation-circle';
    }
  }

  hasShillBidding(suspicions: Suspicion[]): boolean {
    return suspicions.some(s => s.type === 'Shill Bidding');
  }

  hasBidSniping(suspicions: Suspicion[]): boolean {
    return suspicions.some(s => s.type === 'Bid Sniping');
  }

  getShillBiddingConfidence(suspicions: Suspicion[]): number | undefined {
    const shillSuspicion = suspicions.find(s => s.type === 'Shill Bidding');
    return shillSuspicion ? shillSuspicion.confidenceScore : undefined;
  }

  getBidSnipingConfidence(suspicions: Suspicion[]): number | undefined {
    const snipingSuspicion = suspicions.find(s => s.type === 'Bid Sniping');
    return snipingSuspicion ? snipingSuspicion.confidenceScore : undefined;
  }
}
