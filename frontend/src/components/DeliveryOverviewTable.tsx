import React, { useState, useEffect } from 'react';
import { 
  Table, 
  Badge, 
  Button, 
  Space, 
  Modal, 
  Form, 
  Input, 
  Select, 
  DatePicker, 
  InputNumber,
  message,
  Popconfirm,
  Card,
  Row,
  Col 
} from 'antd';
import { 
  PlusOutlined, 
  EditOutlined, 
  DeleteOutlined, 
  ReloadOutlined,
  TruckOutlined 
} from '@ant-design/icons';
import dayjs from 'dayjs';
import { 
  DeliveryOverview, 
  CreateDeliveryRequest, 
  ArriveStatus, 
  ARRIVE_STATUS_OPTIONS 
} from '../types';
import { deliveryAPI } from '../services/api';

const DeliveryOverviewTable: React.FC = () => {
  const [deliveries, setDeliveries] = useState<DeliveryOverview[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalVisible, setModalVisible] = useState(false);
  const [editingRecord, setEditingRecord] = useState<DeliveryOverview | null>(null);
  const [form] = Form.useForm();

  // 載入進貨資料
  const loadDeliveries = async () => {
    setLoading(true);
    try {
      const response = await deliveryAPI.getDeliveries();
      if (response.success && response.data) {
        setDeliveries(response.data);
      } else {
        message.error(response.message || '載入資料失敗');
      }
    } catch (error: any) {
      console.error('載入進貨資料錯誤:', error);
      message.error('載入資料失敗，請檢查網路連線');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadDeliveries();
  }, []);

  // 處理新增/編輯
  const handleSubmit = async (values: any) => {
    try {
      const requestData: CreateDeliveryRequest = {
        blNo: values.blNo,
        customer: values.customer,
        style: values.style,
        poNo: values.poNo,
        rolls: values.rolls,
        etd: values.etd ? values.etd.format('YYYY-MM-DD') : undefined,
        eta: values.eta ? values.eta.format('YYYY-MM-DD') : undefined,
        ftyEta: values.ftyEta ? values.ftyEta.format('YYYY-MM-DD') : undefined,
        arriveStatus: values.arriveStatus
      };

      let response;
      if (editingRecord) {
        response = await deliveryAPI.updateDelivery(editingRecord.id, requestData);
        message.success('更新成功！');
      } else {
        response = await deliveryAPI.createDelivery(requestData);
        message.success('新增成功！');
      }

      if (response.success) {
        setModalVisible(false);
        setEditingRecord(null);
        form.resetFields();
        loadDeliveries();
      }
    } catch (error: any) {
      console.error('提交錯誤:', error);
      message.error(error.response?.data?.message || '操作失敗');
    }
  };

  // 處理刪除
  const handleDelete = async (id: number) => {
    try {
      const response = await deliveryAPI.deleteDelivery(id);
      if (response.success) {
        message.success('刪除成功！');
        loadDeliveries();
      }
    } catch (error: any) {
      console.error('刪除錯誤:', error);
      message.error(error.response?.data?.message || '刪除失敗');
    }
  };

  // 開啟編輯模態框
  const handleEdit = (record: DeliveryOverview) => {
    setEditingRecord(record);
    form.setFieldsValue({
      ...record,
      etd: record.etd ? dayjs(record.etd) : null,
      eta: record.eta ? dayjs(record.eta) : null,
      ftyEta: record.ftyEta ? dayjs(record.ftyEta) : null
    });
    setModalVisible(true);
  };

  // 開啟新增模態框
  const handleAdd = () => {
    setEditingRecord(null);
    form.resetFields();
    setModalVisible(true);
  };

  // 表格欄位定義
  const columns = [
    {
      title: 'BL NO',
      dataIndex: 'blNo',
      key: 'blNo',
      width: 140,
      fixed: 'left' as const,
    },
    {
      title: '客戶名稱',
      dataIndex: 'customer',
      key: 'customer',
      width: 100,
    },
    {
      title: '款式編號',
      dataIndex: 'style',
      key: 'style',
      width: 160,
      render: (text: string) => text || '-',
    },
    {
      title: 'PO NO',
      dataIndex: 'poNo',
      key: 'poNo',
      width: 160,
      render: (text: string) => text || '-',
    },
    {
      title: '布卷數量',
      dataIndex: 'rolls',
      key: 'rolls',
      width: 100,
      align: 'right' as const,
      render: (value: number) => value ? value.toFixed(2) : '-',
    },
    {
      title: 'ETD',
      dataIndex: 'etd',
      key: 'etd',
      width: 110,
      render: (date: string) => date ? dayjs(date).format('YYYY/MM/DD') : '-',
    },
    {
      title: 'ETA',
      dataIndex: 'eta',
      key: 'eta',
      width: 110,
      render: (date: string) => date ? dayjs(date).format('YYYY/MM/DD') : '-',
    },
    {
      title: 'Fty ETA',
      dataIndex: 'ftyEta',
      key: 'ftyEta',
      width: 110,
      render: (date: string) => date ? dayjs(date).format('YYYY/MM/DD') : '-',
    },
    {
      title: '到廠狀態',
      dataIndex: 'arriveStatus',
      key: 'arriveStatus',
      width: 110,
      align: 'center' as const,
      render: (status: ArriveStatus) => (
        <Badge 
          status={status === ArriveStatus.OnTime ? 'success' : 'error'} 
          text={status === ArriveStatus.OnTime ? '如期' : '延遲'} 
        />
      ),
    },
    {
      title: '操作',
      key: 'action',
      width: 120,
      fixed: 'right' as const,
      render: (_: any, record: DeliveryOverview) => (
        <Space size="small">
          <Button 
            type="primary" 
            size="small" 
            icon={<EditOutlined />}
            onClick={() => handleEdit(record)}
          >
            編輯
          </Button>
          <Popconfirm
            title="確定要刪除這筆記錄嗎？"
            onConfirm={() => handleDelete(record.id)}
            okText="確定"
            cancelText="取消"
          >
            <Button 
              danger 
              size="small" 
              icon={<DeleteOutlined />}
            >
              刪除
            </Button>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <Card 
      title={
        <div style={{ display: 'flex', alignItems: 'center' }}>
          <TruckOutlined style={{ marginRight: '8px', color: '#1890ff' }} />
          進貨到廠預估表
        </div>
      }
      extra={
        <Space>
          <Button 
            icon={<ReloadOutlined />} 
            onClick={loadDeliveries}
            loading={loading}
          >
            重新載入
          </Button>
          <Button 
            type="primary" 
            icon={<PlusOutlined />}
            onClick={handleAdd}
          >
            新增記錄
          </Button>
        </Space>
      }
    >
      <Table
        columns={columns}
        dataSource={deliveries}
        rowKey="id"
        loading={loading}
        scroll={{ x: 1200, y: 600 }}
        pagination={{
          pageSize: 10,
          showSizeChanger: true,
          showQuickJumper: true,
          showTotal: (total, range) => 
            `第 ${range[0]}-${range[1]} 筆，共 ${total} 筆記錄`,
        }}
        size="small"
      />

      {/* 新增/編輯模態框 */}
      <Modal
        title={editingRecord ? '編輯進貨記錄' : '新增進貨記錄'}
        open={modalVisible}
        onCancel={() => {
          setModalVisible(false);
          setEditingRecord(null);
          form.resetFields();
        }}
        footer={null}
        width={600}
      >
        <Form
          form={form}
          layout="vertical"
          onFinish={handleSubmit}
        >
          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                label="提單號碼"
                name="blNo"
                rules={[{ required: true, message: '請輸入提單號碼!' }]}
              >
                <Input placeholder="請輸入提單號碼" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                label="客戶名稱"
                name="customer"
                rules={[{ required: true, message: '請輸入客戶名稱!' }]}
              >
                <Input placeholder="請輸入客戶名稱" />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                label="款式編號"
                name="style"
              >
                <Input placeholder="請輸入款式編號" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                label="採購單號"
                name="poNo"
              >
                <Input placeholder="請輸入採購單號" />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                label="布卷數量"
                name="rolls"
              >
                <InputNumber 
                  placeholder="請輸入布卷數量" 
                  min={0}
                  precision={2}
                  style={{ width: '100%' }}
                />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                label="到廠狀態"
                name="arriveStatus"
                rules={[{ required: true, message: '請選擇到廠狀態!' }]}
              >
                <Select placeholder="請選擇到廠狀態">
                  {ARRIVE_STATUS_OPTIONS.map(option => (
                    <Select.Option key={option.value} value={option.value}>
                      {option.label}
                    </Select.Option>
                  ))}
                </Select>
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={8}>
              <Form.Item
                label="預計出貨日 (ETD)"
                name="etd"
              >
                <DatePicker 
                  placeholder="選擇日期" 
                  style={{ width: '100%' }}
                  format="YYYY/MM/DD"
                />
              </Form.Item>
            </Col>
            <Col span={8}>
              <Form.Item
                label="預計到港日 (ETA)"
                name="eta"
              >
                <DatePicker 
                  placeholder="選擇日期" 
                  style={{ width: '100%' }}
                  format="YYYY/MM/DD"
                />
              </Form.Item>
            </Col>
            <Col span={8}>
              <Form.Item
                label="預計到廠日 (Fty ETA)"
                name="ftyEta"
              >
                <DatePicker 
                  placeholder="選擇日期" 
                  style={{ width: '100%' }}
                  format="YYYY/MM/DD"
                />
              </Form.Item>
            </Col>
          </Row>

          <Form.Item style={{ textAlign: 'right', marginBottom: 0, marginTop: 24 }}>
            <Space>
              <Button onClick={() => {
                setModalVisible(false);
                setEditingRecord(null);
                form.resetFields();
              }}>
                取消
              </Button>
              <Button type="primary" htmlType="submit">
                {editingRecord ? '更新' : '新增'}
              </Button>
            </Space>
          </Form.Item>
        </Form>
      </Modal>
    </Card>
  );
};

export default DeliveryOverviewTable; 