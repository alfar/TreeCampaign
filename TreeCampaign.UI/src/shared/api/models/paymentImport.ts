export interface PaymentImportFailure {
  lineNumber: number;
  reason: string;
}

export interface PaymentImportSummary {
  importedCount: number;
  skippedDuplicateTransactionIds: string[];
  failures: PaymentImportFailure[];
}
