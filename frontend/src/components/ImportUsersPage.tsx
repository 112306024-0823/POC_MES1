import React, { useState } from 'react';
import { Card, Upload, Button, Table, message, Typography, Space } from 'antd';
import { UploadOutlined, FileExcelOutlined, FileTextOutlined, ArrowLeftOutlined } from '@ant-design/icons';
import { authAPI } from '../services/api';
import { ImportUserResult, ImportUsersResponse } from '../types';

interface ImportUsersPageProps {
  onBack: () => void;
}

const ImportUsersPage: React.FC<ImportUsersPageProps> = ({ onBack }) => {
  const [uploading, setUploading] = useState(false);
  const [results, setResults] = useState<ImportUserResult[]>([]);

  const handleUpload = async (file: File) => {
    setUploading(true);
    try {
      const response = await authAPI.importUsers(file);
      if (response.success && response.data) {
        setResults(response.data.results);
        message.success('匯入完成！');
      } else {
        message.error(response.message || '匯入失敗');
      }
    } catch (error: any) {
      message.error(error.response?.data?.message || '匯入失敗，請檢查檔案格式');
    } finally {
      setUploading(false);
    }
  };

  const handleDownloadTemplate = async (type: 'xlsx' | 'csv') => {
    try {
      const blob = await authAPI.downloadImportTemplate(type);
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = type === 'csv' ? 'UserImportTemplate.csv' : 'UserImportTemplate.xlsx';
      document.body.appendChild(a);
      a.click();
      a.remove();
      window.URL.revokeObjectURL(url);
    } catch {
      message.error('下載範本失敗');
    }
  };

  const columns = [
    { title: '帳號', dataIndex: 'username', key: 'username' },
    { title: '廠別', dataIndex: 'factory', key: 'factory' },
    { title: '成功', dataIndex: 'success', key: 'success', render: (v: boolean) => v ? '✅' : '❌' },
    { title: '自動密碼', dataIndex: 'generatedPassword', key: 'generatedPassword', render: (v: string) => v || '-' },
    { title: '錯誤訊息', dataIndex: 'error', key: 'error', render: (v: string) => v || '-' },
  ];

  return (
    <Card
      title={<span><ArrowLeftOutlined style={{ marginRight: 8, cursor: 'pointer' }} onClick={onBack} />帳號匯入</span>}
      style={{ maxWidth: 700, margin: '0 auto', boxShadow: '0 4px 12px rgba(0,0,0,0.12)' }}
    >
      <Space direction="vertical" style={{ width: '100%' }}>
        <Typography.Paragraph>
          請上傳 Excel（.xlsx）或 CSV 檔案，欄位：<b>Username</b>、<b>Factory</b>、<b>Password</b>（可空，系統自動產生）。
        </Typography.Paragraph>
        <Space>
          <Button icon={<FileExcelOutlined />} onClick={() => handleDownloadTemplate('xlsx')}>下載 Excel 範本</Button>
          <Button icon={<FileTextOutlined />} onClick={() => handleDownloadTemplate('csv')}>下載 CSV 範本</Button>
        </Space>
        <Upload.Dragger
          name="file"
          accept=".xlsx,.csv"
          customRequest={({ file, onSuccess }) => {
            handleUpload(file as File);
            if (onSuccess) setTimeout(() => onSuccess('ok'), 0);
          }}
          showUploadList={false}
          disabled={uploading}
        >
          <p className="ant-upload-drag-icon">
            <UploadOutlined />
          </p>
          <p className="ant-upload-text">點擊或拖曳檔案到此上傳</p>
          <p className="ant-upload-hint">僅支援 .xlsx 或 .csv</p>
        </Upload.Dragger>
        {results.length > 0 && (
          <Table
            columns={columns}
            dataSource={results.map((r, i) => ({ ...r, key: i }))}
            pagination={false}
            style={{ marginTop: 24 }}
            bordered
            size="small"
          />
        )}
      </Space>
    </Card>
  );
};

export default ImportUsersPage; 