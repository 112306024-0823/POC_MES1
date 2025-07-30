import axios from 'axios';
import { 
  LoginRequest, 
  LoginResponse, 
  DeliveryOverview, 
  CreateDeliveryRequest, 
  ApiResponse,
  Factory,
  RegisterRequest,
  RegisterResponse,
  ImportUser,
  ImportUsersResponse,
  User
} from '../types';

// 建立 axios 實例
const api = axios.create({
  // 部署到 Render 時，請在 Render 靜態站台設定 REACT_APP_API_BASE_URL，指向你的後端 API 服務網址
  baseURL: process.env.REACT_APP_API_BASE_URL || 'http://localhost:5000/api',
  timeout: 10000,
});

// 請求攔截器 - 添加 JWT Token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// 回應攔截器 - 處理錯誤
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Token 過期或無效，清除本地儲存並重導向到登入頁
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// 認證相關 API
export const authAPI = {
  // 登入
  async login(loginData: LoginRequest): Promise<ApiResponse<LoginResponse>> {
    const response = await api.post<ApiResponse<LoginResponse>>('/auth/login', loginData);
    return response.data;
  },

  // 取得當前使用者資訊
  async getCurrentUser(): Promise<ApiResponse<any>> {
    const response = await api.get<ApiResponse<any>>('/auth/me');
    return response.data;
  },

  // 註冊
  async register(registerData: RegisterRequest): Promise<ApiResponse<RegisterResponse>> {
    const response = await api.post<ApiResponse<RegisterResponse>>('/auth/register', registerData);
    return response.data;
  },

  // 帳號匯入
  async importUsers(file: File): Promise<ApiResponse<ImportUsersResponse>> {
    const formData = new FormData();
    formData.append('file', file);
    const response = await api.post<ApiResponse<ImportUsersResponse>>('/auth/import-users', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
    return response.data;
  },

  // 下載範本
  async downloadImportTemplate(type: 'xlsx' | 'csv' = 'xlsx'): Promise<Blob> {
    const response = await api.get(`/auth/import-template?type=${type}`, { responseType: 'blob' });
    return response.data;
  },

  // 取得所有使用者
  async getUsers(): Promise<ApiResponse<User[]>> {
    const response = await api.get<ApiResponse<User[]>>('/auth/users');
    return response.data;
  }
};

// 進貨到廠預估表相關 API
export const deliveryAPI = {
  // 取得進貨資料列表
  async getDeliveries(factory?: Factory): Promise<ApiResponse<DeliveryOverview[]>> {
    const params = factory ? { factory } : {};
    const response = await api.get<ApiResponse<DeliveryOverview[]>>('/delivery', { params });
    return response.data;
  },

  // 根據ID取得進貨資料
  async getDeliveryById(id: number): Promise<ApiResponse<DeliveryOverview>> {
    const response = await api.get<ApiResponse<DeliveryOverview>>(`/delivery/${id}`);
    return response.data;
  },

  // 建立進貨資料
  async createDelivery(data: CreateDeliveryRequest): Promise<ApiResponse<DeliveryOverview>> {
    const response = await api.post<ApiResponse<DeliveryOverview>>('/delivery', data);
    return response.data;
  },

  // 更新進貨資料
  async updateDelivery(id: number, data: CreateDeliveryRequest): Promise<ApiResponse<DeliveryOverview>> {
    const response = await api.put<ApiResponse<DeliveryOverview>>(`/delivery/${id}`, data);
    return response.data;
  },

  // 刪除進貨資料
  async deleteDelivery(id: number): Promise<ApiResponse<any>> {
    const response = await api.delete<ApiResponse<any>>(`/delivery/${id}`);
    return response.data;
  }
};

export const dashboardAPI = {
  async getSummary(): Promise<ApiResponse<any>> {
    const response = await api.get('/dashboard/summary');
    return response.data;
  },
  
  async test(): Promise<ApiResponse<any>> {
    const response = await api.get('/dashboard/test');
    return response.data;
  }
};

export default api; 