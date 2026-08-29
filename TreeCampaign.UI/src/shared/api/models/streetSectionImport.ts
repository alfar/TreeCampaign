export interface StreetSectionImportFailure {
  lineNumber: number;
  reason: string;
}

export interface StreetSectionImportSummary {
  importedCount: number;
  skippedExistingCount: number;
  failures: StreetSectionImportFailure[];
}
