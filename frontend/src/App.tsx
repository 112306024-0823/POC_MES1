import React, { useState, useEffect } from 'react';
import { ConfigProvider, message } from 'antd';
import zhTW from 'antd/locale/zh_TW';
import { User, Factory } from './types';
import LoginPage from './components/LoginPage';
import MainLayout from './components/MainLayout';
import './App.css';

const App: React.FC = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  // 檢查本地儲存的登入狀態
  useEffect(() => {
    const token = localStorage.getItem('token');
    const userData = localStorage.getItem('user');

    if (token && userData) {
      try {
        const parsedUser = JSON.parse(userData);
        // 確保 isAdmin 欄位存在
        if (parsedUser.isAdmin === undefined) {
          parsedUser.isAdmin = false;
        }
        setUser(parsedUser);
        setIsAuthenticated(true);
      } catch (error) {
        console.error('解析使用者資料錯誤:', error);
        handleLogout();
      }
    }
    setLoading(false);
  }, []);

  // 處理登入成功
  const handleLoginSuccess = (token: string, username: string, factory: Factory, isAdmin: boolean) => {
    const userData: User = { username, factory, isAdmin };
    setUser(userData);
    setIsAuthenticated(true);
  };

  // 處理登出
  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setUser(null);
    setIsAuthenticated(false);
    message.success('已成功登出');
  };

  // 載入中
  if (loading) {
    return (
      <div style={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        height: '100vh',
        background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)'
      }}>
        <div style={{ color: '#fff', fontSize: '18px' }}>載入中...</div>
      </div>
    );
  }

  return (
    <ConfigProvider locale={zhTW}>
      <div className="App">
        {isAuthenticated && user ? (
          <MainLayout user={user} onLogout={handleLogout} />
        ) : (
          <LoginPage onLoginSuccess={handleLoginSuccess} />
        )}
      </div>
    </ConfigProvider>
  );
};

export default App; 