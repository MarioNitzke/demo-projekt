import React from 'react';
import { Link } from 'react-router-dom';
import Layout from '@/components/Layout';

export default function HomePage() {
  return (
    <Layout>
      {/* Hero Section */}
      <div className="text-center py-16 md:py-24">
        <div className="inline-flex items-center gap-2 bg-teal-50 text-teal-700 px-4 py-1.5 rounded-full text-sm font-medium mb-6">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            className="h-4 w-4"
            viewBox="0 0 20 20"
            fill="currentColor"
          >
            <path
              fillRule="evenodd"
              d="M3.172 5.172a4 4 0 015.656 0L10 6.343l1.172-1.171a4 4 0 115.656 5.656L10 17.657l-6.828-6.829a4 4 0 010-5.656z"
              clipRule="evenodd"
            />
          </svg>
          Your Health, Our Priority
        </div>
        <h1 className="text-4xl md:text-6xl font-bold text-gray-900 mb-6 leading-tight">
          Welcome to{' '}
          <span className="text-teal-600">PhysioBook</span>
        </h1>
        <p className="text-xl text-gray-600 max-w-2xl mx-auto mb-10">
          Your trusted platform for physiotherapy knowledge and appointment booking.
          Browse expert articles, manage your health journey, and connect with qualified
          physiotherapists.
        </p>
        <div className="flex flex-col sm:flex-row gap-4 justify-center">
          <Link
            to="/articles"
            className="inline-flex items-center justify-center px-8 py-3 text-base font-medium text-white bg-teal-600 rounded-lg hover:bg-teal-700 transition-colors shadow-lg shadow-teal-600/25"
          >
            Browse Articles
            <svg
              xmlns="http://www.w3.org/2000/svg"
              className="ml-2 h-5 w-5"
              viewBox="0 0 20 20"
              fill="currentColor"
            >
              <path
                fillRule="evenodd"
                d="M10.293 3.293a1 1 0 011.414 0l6 6a1 1 0 010 1.414l-6 6a1 1 0 01-1.414-1.414L14.586 11H3a1 1 0 110-2h11.586l-4.293-4.293a1 1 0 010-1.414z"
                clipRule="evenodd"
              />
            </svg>
          </Link>
          <Link
            to="/register"
            className="inline-flex items-center justify-center px-8 py-3 text-base font-medium text-teal-700 bg-teal-50 rounded-lg hover:bg-teal-100 transition-colors"
          >
            Get Started
          </Link>
        </div>
      </div>

      {/* Features Section */}
      <div className="py-16 border-t border-gray-200">
        <h2 className="text-3xl font-bold text-center text-gray-900 mb-12">
          Why Choose PhysioBook?
        </h2>
        <div className="grid md:grid-cols-3 gap-8">
          <div className="bg-white rounded-xl p-8 shadow-sm border border-gray-100 hover:shadow-md transition-shadow">
            <div className="w-12 h-12 bg-teal-100 rounded-lg flex items-center justify-center mb-4">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                className="h-6 w-6 text-teal-600"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253"
                />
              </svg>
            </div>
            <h3 className="text-xl font-semibold text-gray-900 mb-2">Expert Articles</h3>
            <p className="text-gray-600">
              Access a curated library of articles written by certified physiotherapy
              professionals covering a wide range of topics.
            </p>
          </div>

          <div className="bg-white rounded-xl p-8 shadow-sm border border-gray-100 hover:shadow-md transition-shadow">
            <div className="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center mb-4">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                className="h-6 w-6 text-blue-600"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"
                />
              </svg>
            </div>
            <h3 className="text-xl font-semibold text-gray-900 mb-2">Easy Booking</h3>
            <p className="text-gray-600">
              Book appointments with qualified physiotherapists in just a few clicks.
              Manage your schedule effortlessly.
            </p>
          </div>

          <div className="bg-white rounded-xl p-8 shadow-sm border border-gray-100 hover:shadow-md transition-shadow">
            <div className="w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center mb-4">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                className="h-6 w-6 text-purple-600"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z"
                />
              </svg>
            </div>
            <h3 className="text-xl font-semibold text-gray-900 mb-2">Trusted Care</h3>
            <p className="text-gray-600">
              All our physiotherapists are certified professionals. Your health and recovery
              are in safe hands.
            </p>
          </div>
        </div>
      </div>
    </Layout>
  );
}
