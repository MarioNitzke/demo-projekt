// src/pages/PaymentSuccessPage.tsx
import React from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import Layout from '@/components/Layout';

export default function PaymentSuccessPage() {
  const [params] = useSearchParams();
  const sessionId = params.get('session_id'); // pokud chcete dále pracovat se session ID

  return (
    <Layout>
      <div className="max-w-xl mx-auto text-center py-16">
        <h1 className="text-3xl font-bold mb-4">Platba úspěšná</h1>
        <p className="text-gray-700 mb-6">
          Děkujeme za vaši platbu. Vaše rezervace byla úspěšně zaplacena.
        </p>
        <Link
          to="/my-bookings"
          className="inline-flex items-center px-6 py-3 bg-teal-600 text-white rounded-lg hover:bg-teal-700 transition-colors"
        >
          Zobrazit moje rezervace
        </Link>
      </div>
    </Layout>
  );
}
