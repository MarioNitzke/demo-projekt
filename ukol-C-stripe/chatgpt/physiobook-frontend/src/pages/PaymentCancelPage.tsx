import React from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import Layout from '@/components/Layout';

export default function PaymentCancelPage() {
  const [params] = useSearchParams();
  const bookingId = params.get('booking_id');

  return (
    <Layout>
      <div className="max-w-xl mx-auto text-center py-16">
        <h1 className="text-3xl font-bold mb-4">Platba zrušena</h1>
        <p className="text-gray-700 mb-6">
          Platba byla zrušena nebo vypršela. Vaše rezervace (ID: {bookingId ?? 'neznámé'}) nebyla zaplacena.
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
