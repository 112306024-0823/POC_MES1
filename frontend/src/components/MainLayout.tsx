import React, { useState, useEffect } from 'react';
import { Layout, Menu, Avatar, Dropdown, Space, Typography, message } from 'antd';
import {
  TruckOutlined,
  BarChartOutlined,
  CheckCircleOutlined,
  FileTextOutlined,
  UserOutlined,
  LogoutOutlined,
  MenuFoldOutlined,
  MenuUnfoldOutlined,
  HomeOutlined,
  CloseOutlined
} from '@ant-design/icons';
import { User, Factory } from '../types';
import DeliveryOverviewTable from './DeliveryOverviewTable';
import ImportUsersPage from './ImportUsersPage';
import DashboardPage from './DashboardPage';

const { Header, Sider, Content } = Layout;
const { Title, Text } = Typography;

interface MainLayoutProps {
  user: User;
  onLogout: () => void;
}

const MainLayout: React.FC<MainLayoutProps> = ({ user, onLogout }) => {
  const [collapsed, setCollapsed] = useState(false);
  const [selectedKey, setSelectedKey] = useState('1');
  const [showImportUsers, setShowImportUsers] = useState(false);
  const [isMobile, setIsMobile] = useState(false);

  // 在 useEffect 監聽視窗寬度，判斷是否為手機
  useEffect(() => {
    const handleResize = () => setIsMobile(window.innerWidth < 768);
    handleResize();
    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, []);

  // 取得工廠名稱
  const getFactoryName = (factory: Factory) => {
    switch (factory) {
      case Factory.TPL:
        return 'TPL';
      case Factory.NVN:
        return 'NVN';
      case Factory.LR:
        return 'LR';
      default:
        return '未知';
    }
  };

  // 使用者選單
  const userMenu = {
    items: [
      {
        key: 'logout',
        icon: <LogoutOutlined />,
        label: '登出',
        onClick: () => {
          message.success('已登出');
          onLogout();
        },
      },
    ],
  };

  // 側邊選單項目
  const menuItems = [
    {
      key: 'dashboard',
      icon: <HomeOutlined />,
      label: 'Dashboard',
    },
    {
      key: '1',
      icon: <TruckOutlined />,
      label: 'Estimated Incoming Shipment',
    },
    {
      key: '2',
      icon: <BarChartOutlined />,
      label: 'Production Overview',
    },
    {
      key: '3',
      icon: <CheckCircleOutlined />,
      label: 'Quality',
    },
    {
      key: '4',
      icon: <FileTextOutlined />,
      label: 'Reports',
    },
    ...(user.isAdmin ? [{
      key: 'import-users',
      icon: <UserOutlined />,
      label: 'Account Setting',
    }] : [])
  ];

  // 渲染主要內容
  const renderContent = () => {
    if (selectedKey === 'import-users') {
      return <ImportUsersPage onBack={() => {
        setShowImportUsers(false);
        setSelectedKey('dashboard'); // 返回時切換到 dashboard
      }} />;
    }
    switch (selectedKey) {
      case 'dashboard':
        return <DashboardPage />;
      case '1':
        return <DeliveryOverviewTable />;
      case '2':
        return (
          <div style={{ 
            padding: '50px', 
            textAlign: 'center', 
            background: '#f5f5f5', 
            borderRadius: '8px' 
          }}>
            <BarChartOutlined style={{ fontSize: '64px', color: '#ccc', marginBottom: '16px' }} />
            <Title level={3} style={{ color: '#999' }}>Production Overview</Title>
            <Text style={{ color: '#999' }}>此功能尚未實作，敬請期待</Text>
          </div>
        );
      case '3':
        return (
          <div style={{ 
            padding: '50px', 
            textAlign: 'center', 
            background: '#f5f5f5', 
            borderRadius: '8px' 
          }}>
            <CheckCircleOutlined style={{ fontSize: '64px', color: '#ccc', marginBottom: '16px' }} />
            <Title level={3} style={{ color: '#999' }}>Quality</Title>
            <Text style={{ color: '#999' }}>此功能尚未實作，敬請期待</Text>
          </div>
        );
      case '4':
        return (
          <div style={{ 
            padding: '50px', 
            textAlign: 'center', 
            background: '#f5f5f5', 
            borderRadius: '8px' 
          }}>
            <FileTextOutlined style={{ fontSize: '64px', color: '#ccc', marginBottom: '16px' }} />
            <Title level={3} style={{ color: '#999' }}>Reports</Title>
            <Text style={{ color: '#999' }}>此功能尚未實作，敬請期待</Text>
          </div>
        );
      default:
        return <DeliveryOverviewTable />;
    }
  };

  return (
    <Layout style={{ minHeight: '100vh' }}>
      {/* 側邊欄 */}
      <Sider 
        trigger={null} 
        collapsible 
        collapsed={collapsed}
        breakpoint="md"
        collapsedWidth={0}
        onBreakpoint={broken => setCollapsed(broken)}
        style={{ background: '#001529' }}
      >
        {/* Sider 內部最上方，手機展開時顯示關閉icon */}
        {isMobile && !collapsed && (
          <div style={{ display: 'flex', justifyContent: 'flex-end', alignItems: 'center', height: 48, padding: '0 12px' }}>
            <CloseOutlined style={{ color: '#fff', fontSize: 24, cursor: 'pointer' }} onClick={() => setCollapsed(true)} />
          </div>
        )}
        <div style={{ 
          height: '64px', 
          display: 'flex', 
          alignItems: 'center', 
          justifyContent: collapsed ? 'center' : 'flex-start',
          background: 'rgba(255, 255, 255, 0.1)',
          margin: '16px',
          borderRadius: '6px',
          paddingLeft: collapsed ? 0 : 16
        }}>
          <HomeOutlined style={{ color: '#fff', fontSize: '24px' }} />
          {!collapsed && (
            <span style={{ color: '#fff', marginLeft: '12px', fontWeight: 'bold' }}>
              MES 系統
            </span>
          )}
        </div>
        
        <Menu
          theme="dark"
          mode="inline"
          selectedKeys={[selectedKey]}
          items={menuItems}
          onClick={({ key }) => {
            setSelectedKey(key);
            if (key === 'import-users') {
              setShowImportUsers(true);
            } else {
              setShowImportUsers(false);
            }
          }}
          style={{ borderRight: 0 }}
        />
      </Sider>

      {/* 主要內容區域 */}
      <Layout>
        {/* 頂部標題列 */}
        <Header style={{ 
          padding: '0 24px', 
          background: '#fff', 
          display: 'flex', 
          alignItems: 'center',
          justifyContent: 'space-between',
          boxShadow: '0 2px 8px rgba(0,0,0,0.06)'
        }}>
          <div style={{ display: 'flex', alignItems: 'center' }}>
            {/* 收合/展開按鈕 */}
            {React.createElement(collapsed ? MenuUnfoldOutlined : MenuFoldOutlined, {
              style: { fontSize: '18px', cursor: 'pointer', marginRight: 16 },
              onClick: () => setCollapsed(!collapsed),
            })}
            <Title level={4} style={{ margin: '0 0 0 0', color: '#001529' }}>
              {menuItems.find(item => item.key === selectedKey)?.label}
            </Title>
          </div>

          <Space>
            <Text style={{ marginRight: '12px' }}>
              歡迎回來，<strong>{user.username}</strong>
            </Text>
            <Text type="secondary" style={{ marginRight: '12px' }}>
              工廠：{getFactoryName(user.factory)}
            </Text>
            <Dropdown menu={userMenu} placement="bottomRight">
              <div style={{ cursor: 'pointer' }}>
                <Avatar 
                  size="small" 
                  icon={<UserOutlined />} 
                  style={{ backgroundColor: '#1890ff' }}
                />
              </div>
            </Dropdown>
          </Space>
        </Header>

        {/* 內容區域 */}
        <Content style={{ 
          margin: '24px', 
          background: '#fff',
          borderRadius: '8px',
          minHeight: 'calc(100vh - 112px)'
        }}>
          <div style={{ padding: '24px' }}>
            {renderContent()}
          </div>
        </Content>
      </Layout>
    </Layout>
  );
};

export default MainLayout; 