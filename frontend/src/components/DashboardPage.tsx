import React, { useEffect, useState, useMemo } from 'react';
import { Card, Row, Col, Statistic, Table, Tag, Typography, message, Input, Space } from 'antd';
import { UserOutlined, TeamOutlined, FileDoneOutlined, CheckCircleOutlined, CloseCircleOutlined, SearchOutlined } from '@ant-design/icons';
import { User, DeliveryOverview, ArriveStatus } from '../types';
import { authAPI, deliveryAPI } from '../services/api';

const { Title } = Typography;

const cardStyle = {
  borderRadius: 10,
  boxShadow: '0 4px 24px rgba(0,0,0,0.08)',
  background: '#fff',
  minHeight: 120,
  display: 'flex',
  alignItems: 'center',
  padding: '0 4vw', // 響應式 padding
};

const iconCircle = (icon: React.ReactNode, color: string) => (
  <div style={{
    width: 'clamp(36px,6vw,48px)',
    height: 'clamp(36px,6vw,48px)',
    borderRadius: '50%',
    background: color,
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    marginRight: 'clamp(8px,2vw,16px)',
    marginLeft: 0,
    boxShadow: '0 2px 8px rgba(0,0,0,0.08)'
  }}>{icon}</div>
);

const statBlock = (title: string, value: any, loading: boolean, color: string) => (
  <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-start', justifyContent: 'center', height: 'clamp(36px,6vw,48px)', marginLeft: 0 }}>
    <span style={{ fontSize: 'clamp(13px,2.5vw,16px)', color: '#64748b', fontWeight: 500, lineHeight: 1 }}>{title}</span>
    <span style={{ fontSize: 'clamp(20px,4vw,28px)', fontWeight: 700, color, lineHeight: 1.6 }}>{loading ? '--' : value}</span>
  </div>
);
//讓標題在ICON旁邊


const DashboardPage: React.FC = () => {
  const [summary, setSummary] = useState<any>(null);
  const [users, setUsers] = useState<User[]>([]);
  const [deliveries, setDeliveries] = useState<DeliveryOverview[]>([]);
  const [userPage, setUserPage] = useState(1);
  const [userPageSize, setUserPageSize] = useState(5);
  const [deliveryPage, setDeliveryPage] = useState(1);
  const [deliveryPageSize, setDeliveryPageSize] = useState(5);
  const [loading, setLoading] = useState(false);
  const [userSearch, setUserSearch] = useState('');
  const [deliverySearch, setDeliverySearch] = useState('');

  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);
      try {
        const [summaryRes, usersRes, deliveriesRes] = await Promise.all([
          fetch('/api/dashboard/summary', { headers: { Authorization: `Bearer ${localStorage.getItem('token')}` } }).then(r => r.json()),
          authAPI.getUsers(),
          deliveryAPI.getDeliveries()
        ]);
        if (summaryRes.success) setSummary(summaryRes.data);
        if (usersRes.success && usersRes.data) setUsers(usersRes.data);
        if (deliveriesRes.success && deliveriesRes.data) setDeliveries(deliveriesRes.data);
      } catch (e) {
        message.error('載入 Dashboard 資料失敗');
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, []);

  // 搜尋過濾
  const filteredUsers = useMemo(() =>
    users.filter(u =>
      u.username.toLowerCase().includes(userSearch.toLowerCase()) ||
      (['', 'TPL', 'NVN', 'LR'][u.factory] || '').toLowerCase().includes(userSearch.toLowerCase())
    ), [users, userSearch]);

  const filteredDeliveries = useMemo(() =>
    deliveries.filter(d =>
      d.blNo.toLowerCase().includes(deliverySearch.toLowerCase()) ||
      (d.customer || '').toLowerCase().includes(deliverySearch.toLowerCase())
    ), [deliveries, deliverySearch]);

  const userColumns = [
    { title: '帳號', dataIndex: 'username', key: 'username' },
    { title: '廠別', dataIndex: 'factory', key: 'factory', render: (f: number) => ['','TPL','NVN','LR'][f] },
    { title: '管理員', dataIndex: 'isAdmin', key: 'isAdmin', render: (v: boolean) => v ? <Tag color="green">是</Tag> : <Tag color="default">否</Tag> },
  ];
  const deliveryColumns = [
    { title: 'BL NO', dataIndex: 'blNo', key: 'blNo' },
    { title: '客戶', dataIndex: 'customer', key: 'customer' },
    { title: '狀態', dataIndex: 'arriveStatus', key: 'arriveStatus', render: (v: ArriveStatus) => v === ArriveStatus.OnTime ? <Tag color="green">如期</Tag> : <Tag color="red">延遲</Tag> },
    { title: '款式', dataIndex: 'style', key: 'style' },
    { title: 'PO NO', dataIndex: 'poNo', key: 'poNo' },
  ];

  return (
    <div style={{ padding: '4vw', background: '#f6f8fa', minHeight: '110vh' }}>
      <Title level={3} style={{ marginBottom: 32, fontWeight: 700, letterSpacing: 1, fontSize: 'clamp(20px,4vw,32px)' }}>Dashboard</Title>
      <Row gutter={[16, 16]}>
        <Col xs={24} sm={12} md={8} lg={6} xl={5}>
          <Card style={cardStyle} bordered={false}>
            <div style={{ display: 'flex', flexDirection: 'row', alignItems: 'center', width: '100%' }}>
              {iconCircle(<UserOutlined style={{ fontSize: 'clamp(18px,3vw,28px)', color: '#6366f1' }} />, '#e0e7ff')}
              {statBlock('使用者總數', summary?.totalUsers, loading, '#1e293b')}
            </div>
          </Card>
        </Col>
        <Col xs={24} sm={12} md={8} lg={6} xl={5}>
          <Card style={cardStyle} bordered={false}>
            <div style={{ display: 'flex', flexDirection: 'row', alignItems: 'center', width: '100%' }}>
              {iconCircle(<TeamOutlined style={{ fontSize: 'clamp(18px,3vw,28px)', color: '#10b981' }} />, '#d1fae5')}
              {statBlock('管理員數', summary?.adminUsers, loading, '#1e293b')}
            </div>
          </Card>
        </Col>
        <Col xs={24} sm={12} md={8} lg={6} xl={5}>
          <Card style={cardStyle} bordered={false}>
            <div style={{ display: 'flex', flexDirection: 'row', alignItems: 'center', width: '100%' }}>
              {iconCircle(<FileDoneOutlined style={{ fontSize: 'clamp(18px,3vw,28px)', color: '#f59e42' }} />, '#fef3c7')}
              {statBlock('進貨記錄總數', summary?.totalDeliveries, loading, '#1e293b')}
            </div>
          </Card>
        </Col>
        <Col xs={24} sm={12} md={8} lg={6} xl={5}>
          <Card style={cardStyle} bordered={false}>
            <div style={{ display: 'flex', flexDirection: 'row', alignItems: 'center', width: '100%' }}>
              {iconCircle(<CheckCircleOutlined style={{ fontSize: 'clamp(18px,3vw,28px)', color: '#22d3ee' }} />, '#cffafe')}
              {statBlock('如期到貨數', summary?.onTimeDeliveries, loading, '#1e293b')}
            </div>
          </Card>
        </Col>
        <Col xs={24} sm={12} md={8} lg={6} xl={5}>
          <Card style={cardStyle} bordered={false}>
            <div style={{ display: 'flex', flexDirection: 'row', alignItems: 'center', width: '100%' }}>
              {iconCircle(<CloseCircleOutlined style={{ fontSize: 'clamp(18px,3vw,28px)', color: '#ef4444' }} />, '#fee2e2')}
              {statBlock('延遲到貨數', summary?.delayedDeliveries, loading, '#1e293b')}
            </div>
          </Card>
        </Col>
      </Row>
      {/* 響應式表格區塊 */}
      <Row gutter={[32, 32]} style={{ marginTop: 24 }}>
        <Col xs={24} md={12}>
          <Card bordered={false} style={{ borderRadius: 16, boxShadow: '0 2px 12px rgba(0,0,0,0.06)' }}>
            <div style={{ display: 'flex', flexWrap: 'wrap', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
              <span style={{ fontWeight: 600, fontSize: 'clamp(16px,3vw,18px)' }}>使用者清單</span>
              <Input
                allowClear
                prefix={<SearchOutlined />}
                placeholder="搜尋帳號/廠別"
                style={{ width: 'clamp(120px,40vw,220px)', marginTop: 8 }}
                value={userSearch}
                onChange={e => setUserSearch(e.target.value)}
              />
            </div>
            <div style={{ overflowX: 'auto' }}>
              <Table
                columns={userColumns}
                dataSource={filteredUsers}
                rowKey="username"
                pagination={{
                  current: userPage,
                  pageSize: userPageSize,
                  total: filteredUsers.length,
                  onChange: (p, ps) => { setUserPage(p); setUserPageSize(ps); },
                  showSizeChanger: true,
                  showTotal: (total, range) => `第 ${range[0]}-${range[1]} 筆，共 ${total} 筆`,
                  position: ['bottomCenter']
                }}
                loading={loading}
                size="middle"
                bordered
                rowClassName={(_, idx) => idx % 2 === 0 ? 'table-row-light' : 'table-row-dark'}
                style={{ minHeight: 320, minWidth: 400 }}
              />
            </div>
          </Card>
        </Col>
        <Col xs={24} md={12}>
          <Card bordered={false} style={{ borderRadius: 16, boxShadow: '0 2px 12px rgba(0,0,0,0.06)' }}>
            <div style={{ display: 'flex', flexWrap: 'wrap', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
              <span style={{ fontWeight: 600, fontSize: 'clamp(16px,3vw,18px)' }}>進貨清單</span>
              <Input
                allowClear
                prefix={<SearchOutlined />}
                placeholder="搜尋 BL NO/客戶"
                style={{ width: 'clamp(120px,40vw,220px)', marginTop: 8 }}
                value={deliverySearch}
                onChange={e => setDeliverySearch(e.target.value)}
              />
            </div>
            <div style={{ overflowX: 'auto' }}>
              <Table
                columns={deliveryColumns}
                dataSource={filteredDeliveries}
                rowKey="id"
                pagination={{
                  current: deliveryPage,
                  pageSize: deliveryPageSize,
                  total: filteredDeliveries.length,
                  onChange: (p, ps) => { setDeliveryPage(p); setDeliveryPageSize(ps); },
                  showSizeChanger: true,
                  showTotal: (total, range) => `第 ${range[0]}-${range[1]} 筆，共 ${total} 筆`,
                  position: ['bottomCenter']
                }}
                loading={loading}
                size="middle"
                bordered
                rowClassName={(_, idx) => idx % 2 === 0 ? 'table-row-light' : 'table-row-dark'}
                style={{ minHeight: 320, minWidth: 400 }}
              />
            </div>
          </Card>
        </Col>
      </Row>
      {/* 條紋表格樣式 */}
      <style>{`
        .table-row-light { background: #f9fafb; }
        .table-row-dark { background: #fff; }
        .ant-table-thead > tr > th { font-weight: 700; background: #f1f5f9; }
      `}</style>
    </div>
  );
};

export default DashboardPage; 