export interface CurrentUser {
  userId: string;
  email: string;
  displayName: string;
  scoutGroupId: string;
  isPlatformAdmin: boolean;
}

export interface User {
  id: string;
  email: string;
  displayName: string;
}
