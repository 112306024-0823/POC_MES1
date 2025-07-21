// 工廠類型
export enum Factory {
  TPL = 1,
  NVN = 2,
  LR = 3
}

// 到廠狀態
export enum ArriveStatus {
  OnTime = 1,
  Delayed = 2
}

// 使用者介面
export interface User {
  username: string;
  factory: Factory;
}

// 登入請求
export interface LoginRequest {
  username: string;
  password: string;
  factory: Factory;
}

// 登入回應
export interface LoginResponse {
  token: string;
  username: string;
  factory: Factory;
  expiresAt: string;
  isAdmin: boolean;
}

// 進貨到廠預估表
export interface DeliveryOverview {
  id: number;
  blNo: string;
  customer: string;
  style?: string;
  poNo?: string;
  rolls?: number;
  etd?: string;
  eta?: string;
  ftyEta?: string;
  arriveStatus: ArriveStatus;
  factory: Factory;
}

// 建立進貨資料請求
export interface CreateDeliveryRequest {
  blNo: string;
  customer: string;
  style?: string;
  poNo?: string;
  rolls?: number;
  etd?: string;
  eta?: string;
  ftyEta?: string;
  arriveStatus: ArriveStatus;
}

// 註冊請求
export interface RegisterRequest {
  username: string;
  password?: string; // 可選，未填則後端自動產生
  factory: Factory;
}

// 註冊回應
export interface RegisterResponse {
  username: string;
  factory: Factory;
  generatedPassword?: string; // 若自動產生密碼則回傳
  isAdmin: boolean;
}

// API 回應格式
export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message: string;
  errorCode?: string;
}

// 工廠選項
export const FACTORY_OPTIONS = [
  { label: 'TPL', value: Factory.TPL },
  { label: 'NVN', value: Factory.NVN },
  { label: 'LR', value: Factory.LR }
];

// 到廠狀態選項
export const ARRIVE_STATUS_OPTIONS = [
  { label: '如期或已到貨', value: ArriveStatus.OnTime },
  { label: '延遲或尚未到貨', value: ArriveStatus.Delayed }
];

// 帳號匯入
export interface ImportUser {
  username: string;
  password?: string;
  factory: Factory;
}
export interface ImportUserResult {
  username: string;
  factory: Factory;
  success: boolean;
  generatedPassword?: string;
  error?: string;
}
export interface ImportUsersResponse {
  results: ImportUserResult[];
} 