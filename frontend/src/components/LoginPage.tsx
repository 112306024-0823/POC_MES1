import React, { useState } from 'react';
import { Form, Input, Button, Select, Card, message, Row, Col } from 'antd';
import { UserOutlined, LockOutlined, HomeOutlined, SafetyCertificateOutlined } from '@ant-design/icons';
import { LoginRequest, RegisterRequest, FACTORY_OPTIONS } from '../types';
import { authAPI } from '../services/api';

interface LoginPageProps {
  onLoginSuccess: (token: string, username: string, factory: number) => void;
}

const RegisterPage: React.FC<{ onBackToLogin: () => void }> = ({ onBackToLogin }) => {
  const [loading, setLoading] = useState(false);

  const handleRegister = async (values: RegisterRequest & { confirmPassword: string }) => {
    setLoading(true);
    try {
      if (values.password && values.password !== values.confirmPassword) {
        message.error('兩次密碼輸入不一致');
        setLoading(false);
        return;
      }
      const { confirmPassword, ...registerData } = values;
      const response = await authAPI.register(registerData);
      if (response.success && response.data) {
        if (response.data.generatedPassword) {
          message.success(`註冊成功！系統已自動產生密碼：${response.data.generatedPassword}`);
        } else {
          message.success('註冊成功！請使用您設定的密碼登入');
        }
        onBackToLogin();
      } else {
        message.error(response.message || '註冊失敗');
      }
    } catch (error: any) {
      message.error(error.response?.data?.message || '註冊失敗，請檢查網路連線');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Card
      title={
        <div style={{ textAlign: 'center', fontSize: '24px', fontWeight: 'bold' }}>
          <SafetyCertificateOutlined style={{ marginRight: '10px', color: '#52c41a' }} />
          MES 系統註冊
        </div>
      }
      style={{ boxShadow: '0 4px 12px rgba(0,0,0,0.15)', borderRadius: '12px' }}
    >
      <Form
        name="register"
        initialValues={{ factory: 1 }}
        onFinish={handleRegister}
        size="large"
        layout="vertical"
      >
        <Form.Item
          label="帳號"
          name="username"
          rules={[{ required: true, message: '請輸入帳號!' }, { min: 3, message: '帳號至少 3 字' }]}
        >
          <Input prefix={<UserOutlined />} placeholder="請輸入帳號" autoComplete="username" />
        </Form.Item>
        <Form.Item
          label="密碼（可留空自動產生）"
          name="password"
          rules={[{ min: 6, message: '密碼至少 6 字' }]}
        >
          <Input.Password prefix={<LockOutlined />} placeholder="請輸入密碼" autoComplete="new-password" />
        </Form.Item>
        <Form.Item
          label="確認密碼"
          name="confirmPassword"
          dependencies={["password"]}
          rules={[
            ({ getFieldValue }) => ({
              validator(_, value) {
                if (!getFieldValue('password') || getFieldValue('password') === value) {
                  return Promise.resolve();
                }
                return Promise.reject(new Error('兩次密碼輸入不一致'));
              },
            }),
          ]}
        >
          <Input.Password prefix={<LockOutlined />} placeholder="請再次輸入密碼" autoComplete="new-password" />
        </Form.Item>
        <Form.Item
          label="廠別"
          name="factory"
          rules={[{ required: true, message: '請選擇廠別!' }]}
        >
          <Select placeholder="請選擇廠別">
            {FACTORY_OPTIONS.map(option => (
              <Select.Option key={option.value} value={option.value}>
                {option.label}
              </Select.Option>
            ))}
          </Select>
        </Form.Item>
        <Form.Item style={{ marginBottom: '8px' }}>
          <Button type="primary" htmlType="submit" loading={loading} block style={{ height: '45px', fontSize: '16px', fontWeight: 'bold' }}>
            註冊
          </Button>
        </Form.Item>
        <Button type="link" block onClick={onBackToLogin}>
          已有帳號？返回登入
        </Button>
      </Form>
    </Card>
  );
};

const LoginPage: React.FC<LoginPageProps> = ({ onLoginSuccess }) => {
  const [loading, setLoading] = useState(false);
  const [showRegister, setShowRegister] = useState(false);

  const handleSubmit = async (values: LoginRequest) => {
    setLoading(true);
    try {
      const response = await authAPI.login(values);
      
      if (response.success && response.data) {
        const { token, username, factory, isAdmin } = response.data;
        
        // 儲存到本地儲存
        localStorage.setItem('token', token);
        localStorage.setItem('user', JSON.stringify({ username, factory, isAdmin }));
        
        message.success('登入成功！');
        onLoginSuccess(token, username, factory);
      } else {
        message.error(response.message || '登入失敗');
      }
    } catch (error: any) {
      console.error('登入錯誤:', error);
      message.error(error.response?.data?.message || '登入失敗，請檢查網路連線');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ 
      minHeight: '100vh', 
      background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      padding: '20px'
    }}>
      <Row justify="center" style={{ width: '100%' }}>
        <Col xs={22} sm={16} md={12} lg={8} xl={6}>
          {showRegister ? (
            <RegisterPage onBackToLogin={() => setShowRegister(false)} />
          ) : (
            <Card
              title={
                <div style={{ textAlign: 'center', fontSize: '24px', fontWeight: 'bold' }}>
                  <HomeOutlined style={{ marginRight: '10px', color: '#1890ff' }} />
                  MES 系統登入
                </div>
              }
              style={{
                boxShadow: '0 4px 12px rgba(0,0,0,0.15)',
                borderRadius: '12px'
              }}
            >
              <Form
                name="login"
                initialValues={{ factory: 1 }}
                onFinish={handleSubmit}
                size="large"
                layout="vertical"
              >
                <Form.Item
                  label="帳號"
                  name="username"
                  rules={[{ required: true, message: '請輸入帳號!' }]}
                >
                  <Input 
                    prefix={<UserOutlined />} 
                    placeholder="請輸入帳號" 
                    autoComplete="username"
                  />
                </Form.Item>

                <Form.Item
                  label="密碼"
                  name="password"
                  rules={[{ required: true, message: '請輸入密碼!' }]}
                >
                  <Input.Password 
                    prefix={<LockOutlined />} 
                    placeholder="請輸入密碼"
                    autoComplete="current-password"
                  />
                </Form.Item>

                <Form.Item
                  label="廠別"
                  name="factory"
                  rules={[{ required: true, message: '請選擇廠別!' }]}
                >
                  <Select placeholder="請選擇廠別">
                    {FACTORY_OPTIONS.map(option => (
                      <Select.Option key={option.value} value={option.value}>
                        {option.label}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>

                <Form.Item style={{ marginBottom: '8px' }}>
                  <Button 
                    type="primary" 
                    htmlType="submit" 
                    loading={loading}
                    block
                    style={{ 
                      height: '45px',
                      fontSize: '16px',
                      fontWeight: 'bold'
                    }}
                  >
                    登入
                  </Button>
                </Form.Item>
              </Form>
              <Button type="link" block onClick={() => setShowRegister(true)}>
                沒有帳號？前往註冊
              </Button>
              {/* 測試帳號說明 */}
              <div style={{ 
                marginTop: '20px', 
                padding: '12px', 
                background: '#f6f8fa', 
                borderRadius: '6px',
                fontSize: '12px',
                color: '#666'
              }}>
                <div style={{ fontWeight: 'bold', marginBottom: '4px' }}>測試帳號：</div>
                <div>帳號: admin, 密碼: 123456, 廠別: TPL</div>
                <div>帳號: user1, 密碼: 123456, 廠別: NVN</div>
                <div>帳號: user2, 密碼: 123456, 廠別: LR</div>
              </div>
            </Card>
          )}
        </Col>
      </Row>
    </div>
  );
};

export default LoginPage; 